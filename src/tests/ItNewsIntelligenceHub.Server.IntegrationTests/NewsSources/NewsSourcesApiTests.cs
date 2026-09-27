using System.Net;
using System.Net.Http.Json;
using ItNewsIntelligenceHub.Server.IntegrationTests.Infrastructure;

namespace ItNewsIntelligenceHub.Server.IntegrationTests.NewsSources;

public sealed class NewsSourcesApiTests(
    CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task CreateNewsSource_WhenRequestIsValid_ReturnsCreatedSource()
    {
        var feedUrl = CreateUniqueFeedUrl();
        // Arrange
        var request = new CreateNewsSourceRequest(
            Name: "Microsoft .NET Blog",
            FeedUrl: feedUrl,
            WebsiteUrl: "https://example.test/",
            Category: "DotNet",
            IsActive: true);

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/news-sources",
            request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdSource = await response.Content
            .ReadFromJsonAsync<NewsSourceResponse>();

        Assert.NotNull(createdSource);
        Assert.Equal(request.Name, createdSource.Name);
        Assert.Equal(request.FeedUrl, createdSource.FeedUrl);
        Assert.Equal(request.Category, createdSource.Category);
        Assert.True(createdSource.IsActive);
    }

    [Fact]
    public async Task GetNewsSources_WhenSourceExists_ReturnsSource()
    {
        // Arrange
        var request = new CreateNewsSourceRequest(
            Name: "GitHub Blog",
            FeedUrl: "https://github.blog/feed/",
            WebsiteUrl: "https://github.blog/",
            Category: "DevOps",
            IsActive: true);

        await _client.PostAsJsonAsync("/api/news-sources", request);

        // Act
        var response = await _client.GetAsync("/api/news-sources");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var sources = await response.Content
            .ReadFromJsonAsync<List<NewsSourceResponse>>();

        Assert.NotNull(sources);
        Assert.Contains(
            sources,
            source => source.FeedUrl == request.FeedUrl);
    }

    [Fact]
    public async Task CreateNewsSource_WhenFeedUrlAlreadyExists_ReturnsConflict()
    {
        var feedUrl = CreateUniqueFeedUrl();

        // Arrange
        var request = new CreateNewsSourceRequest(
            Name: "Microsoft .NET Blog",
            FeedUrl: feedUrl,
            WebsiteUrl: "https://example.test/",
            Category: "DotNet",
            IsActive: true);

        await _client.PostAsJsonAsync("/api/news-sources", request);

        // Act
        var duplicateRequest = request with
        {
            Name = "Same feed under another name"
        };

        var response = await _client.PostAsJsonAsync(
            "/api/news-sources",
            duplicateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task FetchNewsSource_WhenSourceDoesNotExist_ReturnsNotFoundProblemDetails()
    {
        // Act
        var response = await _client.PostAsync(
            $"/api/news-sources/{Guid.NewGuid()}/fetch",
            content: null);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var problem = await response.Content
            .ReadFromJsonAsync<ProblemDetailsResponse>();

        Assert.NotNull(problem);
        Assert.Equal(404, problem.Status);
        Assert.Equal("Resource not found", problem.Title);
        Assert.False(string.IsNullOrWhiteSpace(problem.TraceId));
    }

    [Fact]
    public async Task HealthCheck_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Healthy", content);
    }

    private sealed record CreateNewsSourceRequest(
        string Name,
        string FeedUrl,
        string? WebsiteUrl,
        string Category,
        bool IsActive);

    private sealed record NewsSourceResponse(
        Guid Id,
        string Name,
        string FeedUrl,
        string? WebsiteUrl,
        string Category,
        bool IsActive,
        DateTime CreatedAtUtc,
        DateTime? LastFetchedAtUtc,
        DateTime? LastSuccessfulFetchAtUtc,
        DateTime? LastFetchAttemptAtUtc,
        string? LastFetchError);

    private sealed record ProblemDetailsResponse(
        string? Type,
        string? Title,
        int? Status,
        string? Detail,
        string? Instance,
        string? TraceId);

    private static string CreateUniqueFeedUrl()
    {
        return $"https://example.test/feeds/{Guid.NewGuid():N}.xml";
    }
}
