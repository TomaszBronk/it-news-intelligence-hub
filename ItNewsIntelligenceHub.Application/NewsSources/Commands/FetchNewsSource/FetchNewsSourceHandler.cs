using System.Security.Cryptography;
using System.Text;
using ItNewsIntelligenceHub.Application.Abstractions.Feeds;
using ItNewsIntelligenceHub.Application.Abstractions.Persistence;
using ItNewsIntelligenceHub.Application.Abstractions.Time;
using ItNewsIntelligenceHub.Domain.Entities;

namespace ItNewsIntelligenceHub.Application.NewsSources.Commands.FetchNewsSource;

public sealed class FetchNewsSourceHandler(
    INewsSourceRepository newsSourceRepository,
    INewsItemRepository newsItemRepository,
    IUnitOfWork unitOfWork,
    IRssFeedReader rssFeedReader,
    IClock clock) : IFetchNewsSourceHandler
{
    public async Task<FetchNewsSourceResult> HandleAsync(
        FetchNewsSourceCommand command,
        CancellationToken cancellationToken)
    {
        var source = await newsSourceRepository.GetByIdAsync(
            command.SourceId,
            cancellationToken)
            ?? throw new KeyNotFoundException(
                $"News source with ID '{command.SourceId}' was not found.");

        if (!source.IsActive)
        {
            throw new InvalidOperationException(
                $"News source '{source.Name}' is inactive and cannot be fetched.");
        }

        if (!Uri.TryCreate(source.FeedUrl, UriKind.Absolute, out var feedUrl)
            || (feedUrl.Scheme != Uri.UriSchemeHttp
                && feedUrl.Scheme != Uri.UriSchemeHttps))
        {
            throw new InvalidOperationException(
                $"News source '{source.Name}' has an invalid feed URL.");
        }

        var fetchedItems = await rssFeedReader.ReadAsync(feedUrl, cancellationToken);

        var existingItems = await newsItemRepository.GetBySourceIdAsync(
            source.Id,
            cancellationToken);

        var existingExternalIds = existingItems
            .Select(item => item.ExternalId)
            .ToHashSet(StringComparer.Ordinal);

        var existingOriginalUrls = existingItems
            .Select(item => item.OriginalUrl)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var itemsToAdd = new List<NewsItem>();
        var skippedItemsCount = 0;

        foreach (var fetchedItem in fetchedItems)
        {
            if (existingExternalIds.Contains(fetchedItem.ExternalId)
                || existingOriginalUrls.Contains(fetchedItem.OriginalUrl))
            {
                skippedItemsCount++;
                continue;
            }

            var newsItem = new NewsItem
            {
                SourceId = source.Id,
                ExternalId = Truncate(fetchedItem.ExternalId, 512)!,
                Title = Truncate(fetchedItem.Title, 500)!,
                Summary = Truncate(fetchedItem.Summary, 8000),
                OriginalUrl = Truncate(fetchedItem.OriginalUrl, 2048)!,
                Author = Truncate(fetchedItem.Author, 250),
                PublishedAtUtc = fetchedItem.PublishedAtUtc,
                RetrievedAtUtc = clock.UtcNowOffset,
                ContentHash = CreateSha256Hash(
                    $"{fetchedItem.Title}|{fetchedItem.OriginalUrl}|{fetchedItem.PublishedAtUtc:O}"),
                Category = source.Category
            };

            itemsToAdd.Add(newsItem);

            existingExternalIds.Add(fetchedItem.ExternalId);
            existingOriginalUrls.Add(fetchedItem.OriginalUrl);
        }

        await newsItemRepository.AddRangeAsync(itemsToAdd, cancellationToken);

        source.LastFetchedAtUtc = clock.UtcNow;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new FetchNewsSourceResult(
            source.Id,
            source.Name,
            fetchedItems.Count,
            itemsToAdd.Count,
            skippedItemsCount,
            clock.UtcNowOffset);
    }

    private static string CreateSha256Hash(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));

        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    private static string? Truncate(string? value, int maximumLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        return value.Length <= maximumLength
            ? value
            : value[..maximumLength];
    }
}