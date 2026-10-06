using ItNewsIntelligenceHub.Application.Abstractions.Feeds;
using ItNewsIntelligenceHub.Application.Abstractions.Persistence;
using ItNewsIntelligenceHub.Application.Abstractions.Time;
using ItNewsIntelligenceHub.Application.Common.Exceptions;
using ItNewsIntelligenceHub.Application.NewsSources.Commands.FetchNewsSource;
using ItNewsIntelligenceHub.Domain.Entities;
using NSubstitute;

namespace ItNewsIntelligenceHub.Application.Tests.NewsSources.Commands.FetchNewsSource;

public class FetchNewsSourceHandlerTests
{
    private static readonly DateTime UtcNow = new(2026, 9, 27, 18, 0, 0, DateTimeKind.Utc);

    private static readonly DateTimeOffset UtcNowOffset =
        new(2026, 9, 27, 18, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task HandleAsync_WhenFeedContainsNewItems_ImportsItemsAndReturnsResult()
    {
        // Arrange
        var source = CreateActiveSource();
        var fetchedItems = new[]
        {
            CreateFeedNewsItem(
                externalId: "entry-1",
                title: "First news item",
                originalUrl: "https://example.com/news/first"),
            CreateFeedNewsItem(
                externalId: "entry-2",
                title: "Second news item",
                originalUrl: "https://example.com/news/second")
        };

        var fixture = CreateFixture(source, existingItems: [], fetchedItems);

        // Act
        var result = await fixture.Handler.HandleAsync(
            new FetchNewsSourceCommand(source.Id),
            CancellationToken.None);

        // Assert
        Assert.Equal(source.Id, result.SourceId);
        Assert.Equal(source.Name, result.SourceName);
        Assert.Equal(2, result.TotalItemsRead);
        Assert.Equal(2, result.ImportedItemsCount);
        Assert.Equal(0, result.SkippedItemsCount);
        Assert.Equal(UtcNowOffset, result.ImportedAtUtc);

        await fixture.NewsItemRepository.Received(1)
            .AddRangeAsync(
                Arg.Is<IEnumerable<NewsItem>>(items => items.Count() == 2),
                CancellationToken.None);

        await fixture.UnitOfWork.Received(1)
            .SaveChangesAsync(CancellationToken.None);

        Assert.Equal(UtcNow, source.LastFetchedAtUtc);
    }

    [Fact]
    public async Task HandleAsync_WhenFeedContainsExistingItems_SkipsDuplicates()
    {
        // Arrange
        var source = CreateActiveSource();

        var existingItem = new NewsItem
        {
            SourceId = source.Id,
            ExternalId = "existing-entry",
            OriginalUrl = "https://example.com/news/existing"
        };

        var fetchedItems = new[]
        {
            CreateFeedNewsItem(
                externalId: "existing-entry",
                title: "Existing item",
                originalUrl: "https://example.com/news/existing"),
            CreateFeedNewsItem(
                externalId: "new-entry",
                title: "New item",
                originalUrl: "https://example.com/news/new")
        };

        var fixture = CreateFixture(
            source,
            existingItems: [existingItem],
            fetchedItems);

        // Act
        var result = await fixture.Handler.HandleAsync(
            new FetchNewsSourceCommand(source.Id),
            CancellationToken.None);

        // Assert
        Assert.Equal(2, result.TotalItemsRead);
        Assert.Equal(1, result.ImportedItemsCount);
        Assert.Equal(1, result.SkippedItemsCount);

        await fixture.NewsItemRepository.Received(1)
            .AddRangeAsync(
                Arg.Is<IEnumerable<NewsItem>>(items =>
                    items.Count() == 1
                    && items.Single().ExternalId == "new-entry"),
                CancellationToken.None);

        await fixture.UnitOfWork.Received(1)
            .SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async Task HandleAsync_WhenSourceIsInactive_ThrowsInvalidOperationException()
    {
        // Arrange
        var source = CreateActiveSource();
        source.IsActive = false;

        var fixture = CreateFixture(
            source,
            existingItems: [],
            fetchedItems: []);

        // Act
        var action = () => fixture.Handler.HandleAsync(
            new FetchNewsSourceCommand(source.Id),
            CancellationToken.None);

        // Assert
        var exception = await Assert.ThrowsAsync<ConflictException>(action);

        Assert.Contains("inactive", exception.Message, StringComparison.OrdinalIgnoreCase);

        await fixture.RssFeedReader.DidNotReceive()
            .ReadAsync(Arg.Any<Uri>(), Arg.Any<CancellationToken>());

        await fixture.NewsItemRepository.DidNotReceive()
            .AddRangeAsync(
                Arg.Any<IEnumerable<NewsItem>>(),
                Arg.Any<CancellationToken>());

        await fixture.UnitOfWork.DidNotReceive()
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenSourceDoesNotExist_ThrowsKeyNotFoundException()
    {
        // Arrange
        var sourceId = Guid.NewGuid();

        var fixture = CreateFixture(
            source: null,
            existingItems: [],
            fetchedItems: []);

        // Act
        var action = () => fixture.Handler.HandleAsync(
            new FetchNewsSourceCommand(sourceId),
            CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(action);

        await fixture.RssFeedReader.DidNotReceive()
            .ReadAsync(Arg.Any<Uri>(), Arg.Any<CancellationToken>());

        await fixture.UnitOfWork.DidNotReceive()
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenFeedItemHasDuplicateUrl_SkipsItem()
    {
        // Arrange
        var source = CreateActiveSource();

        var existingItem = new NewsItem
        {
            SourceId = source.Id,
            ExternalId = "different-id",
            OriginalUrl = "https://example.com/news/same-url"
        };

        var fetchedItems = new[]
        {
            CreateFeedNewsItem(
                externalId: "new-id",
                title: "Same URL but different external ID",
                originalUrl: "https://example.com/news/same-url")
        };

        var fixture = CreateFixture(
            source,
            existingItems: [existingItem],
            fetchedItems);

        // Act
        var result = await fixture.Handler.HandleAsync(
            new FetchNewsSourceCommand(source.Id),
            CancellationToken.None);

        // Assert
        Assert.Equal(0, result.ImportedItemsCount);
        Assert.Equal(1, result.SkippedItemsCount);

        await fixture.NewsItemRepository.DidNotReceive()
           .AddRangeAsync(
               Arg.Any<IEnumerable<NewsItem>>(),
               Arg.Any<CancellationToken>());
    }

    private static TestFixture CreateFixture(
        NewsSource? source,
        IReadOnlyList<NewsItem> existingItems,
        IReadOnlyCollection<FeedNewsItem> fetchedItems)
    {
        var newsSourceRepository = Substitute.For<INewsSourceRepository>();
        var newsItemRepository = Substitute.For<INewsItemRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var rssFeedReader = Substitute.For<IRssFeedReader>();
        var clock = Substitute.For<IClock>();

        newsSourceRepository
            .GetByIdAsync(
                Arg.Any<Guid>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(source));

        newsItemRepository
            .GetBySourceIdAsync(
                Arg.Any<Guid>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(existingItems));

        rssFeedReader
            .ReadAsync(
                Arg.Any<Uri>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(fetchedItems));

        clock.UtcNow.Returns(UtcNow);
        clock.UtcNowOffset.Returns(UtcNowOffset);

        var handler = new FetchNewsSourceHandler(
            newsSourceRepository,
            newsItemRepository,
            unitOfWork,
            rssFeedReader,
            clock);

        return new TestFixture(
            handler,
            newsItemRepository,
            unitOfWork,
            rssFeedReader);
    }

    private static NewsSource CreateActiveSource()
    {
        return new NewsSource
        {
            Id = Guid.NewGuid(),
            Name = "Test feed",
            FeedUrl = "https://example.com/feed.xml",
            Category = "DotNet",
            IsActive = true
        };
    }

    private static FeedNewsItem CreateFeedNewsItem(
        string externalId,
        string title,
        string originalUrl)
    {
        return new FeedNewsItem(
            externalId,
            title,
            Summary: "Test summary",
            originalUrl,
            Author: "Test author",
            PublishedAtUtc: new DateTimeOffset(
                2026,
                9,
                27,
                12,
                0,
                0,
                TimeSpan.Zero));
    }

    private sealed record TestFixture(
        FetchNewsSourceHandler Handler,
        INewsItemRepository NewsItemRepository,
        IUnitOfWork UnitOfWork,
        IRssFeedReader RssFeedReader);
}