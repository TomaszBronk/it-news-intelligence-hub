using ItNewsIntelligenceHub.Domain.Entities;
using ItNewsIntelligenceHub.Domain.Enums;
using ItNewsIntelligenceHub.Server.IntegrationTests.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace ItNewsIntelligenceHub.Server.IntegrationTests.NewsItems;

public sealed class NewsItemsApiTests(
    CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task UpdateStatus_WhenItemExists_ReturnsUpdatedItemWithStringStatus()
    {
        // Arrange
        var source = CreateSource();
        var item = CreateNewsItem(
            source,
            title: "Azure update",
            status: NewsItemStatus.New);

        await factory.SeedAsync(source, item);

        var request = new UpdateNewsItemStatusRequest("Saved");

        // Act
        var response = await _client.PatchAsJsonAsync(
            $"/api/news-items/{item.Id}/status",
            request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseJson = await response.Content.ReadAsStringAsync();

        Assert.Contains("\"status\":\"Saved\"", responseJson);

        var updatedItem = await response.Content
            .ReadFromJsonAsync<NewsItemResponse>();

        Assert.NotNull(updatedItem);
        Assert.Equal(item.Id, updatedItem.Id);
        Assert.Equal("Saved", updatedItem.Status);
    }


    [Fact]
    public async Task UpdateStatus_WhenItemDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var request = new UpdateNewsItemStatusRequest("Read");

        // Act
        var response = await _client.PatchAsJsonAsync(
            $"/api/news-items/{Guid.NewGuid()}/status",
            request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static NewsSource CreateSource()
    {
        var sourceId = Guid.NewGuid();

        return new NewsSource
        {
            Id = sourceId,
            Name = $"Test Source {sourceId:N}",
            FeedUrl = $"https://example.test/{sourceId:N}/feed.xml",
            WebsiteUrl = "https://example.test/",
            Category = "DotNet",
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    private static NewsItem CreateNewsItem(
        NewsSource source,
        string title,
        NewsItemStatus status)
    {
        var itemId = Guid.NewGuid();

        return new NewsItem
        {
            Id = itemId,
            SourceId = source.Id,
            Source = source,
            ExternalId = $"entry-{itemId:N}",
            Title = title,
            Summary = $"Summary for {title}",
            OriginalUrl = $"https://example.test/news/{itemId:N}",
            Author = "Integration Test",
            PublishedAtUtc = DateTimeOffset.UtcNow,
            RetrievedAtUtc = DateTimeOffset.UtcNow,
            ContentHash = Guid.NewGuid().ToString("N"),
            Category = source.Category,
            Status = status
        };
    }

    [Fact]
    public async Task UpdateNote_WhenItemExists_ReturnsItemWithUpdatedNote()
    {
        // Arrange
        var source = CreateSource();

        var item = CreateNewsItem(
            source,
            title: "Azure AI Search updates",
            status: NewsItemStatus.Saved);

        await factory.SeedAsync(source, item);

        var request = new UpdateNewsItemNoteRequest(
            "Review Azure AI Search documentation before implementation.");

        // Act
        var response = await _client.PatchAsJsonAsync(
            $"/api/news-items/{item.Id}/note",
            request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updatedItem = await response.Content
            .ReadFromJsonAsync<NewsItemResponse>();

        Assert.NotNull(updatedItem);
        Assert.Equal(
            "Review Azure AI Search documentation before implementation.",
            updatedItem.Note);
    }

    private sealed record UpdateNewsItemStatusRequest(
        string Status);

    private sealed record UpdateNewsItemNoteRequest(
        string? Note);

    private sealed record NewsItemResponse(
        Guid Id,
        Guid SourceId,
        string SourceName,
        string Title,
        string? Summary,
        string OriginalUrl,
        string? Author,
        DateTimeOffset? PublishedAtUtc,
        DateTimeOffset RetrievedAtUtc,
        string Category,
        string Status,
        string Note);


}