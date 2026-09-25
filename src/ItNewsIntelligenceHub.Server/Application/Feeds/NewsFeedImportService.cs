using ItNewsIntelligenceHub.Server.Domain.Entities;
using ItNewsIntelligenceHub.Server.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ItNewsIntelligenceHub.Server.Application.Feeds;

public class NewsFeedImportService(
    NewsHubDbContext dbContext,
    IRssFeedReader rssFeedReader,
    ILogger<NewsFeedImportService> logger) : INewsFeedImportService
{
    public async Task<FeedImportResult> ImportAsync(
        Guid sourceId,
        CancellationToken cancellationToken)
    {
        var source = await dbContext.NewsSources
            .SingleOrDefaultAsync(item => item.Id == sourceId, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"News source with ID '{sourceId}' was not found.");

        if (!source.IsActive)
        {
            throw new InvalidOperationException(
                $"News source '{source.Name}' is inactive and cannot be fetched.");
        }

        if (!Uri.TryCreate(source.FeedUrl, UriKind.Absolute, out var feedUrl)
            || (feedUrl.Scheme != Uri.UriSchemeHttp && feedUrl.Scheme != Uri.UriSchemeHttps))
        {
            throw new InvalidOperationException(
                $"News source '{source.Name}' has an invalid feed URL.");
        }

        var fetchedItems = await rssFeedReader.ReadAsync(feedUrl, cancellationToken);

        var existingExternalIds = await dbContext.NewsItems
            .Where(item => item.SourceId == sourceId)
            .Select(item => item.ExternalId)
            .ToHashSetAsync(cancellationToken);

        var existingOriginalUrls = await dbContext.NewsItems
            .Where(item => item.SourceId == sourceId)
            .Select(item => item.OriginalUrl)
            .ToHashSetAsync(cancellationToken);

        var importedItemsCount = 0;
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
                Summary = Truncate(fetchedItem.Summary, 8000)!,
                OriginalUrl = Truncate(fetchedItem.OriginalUrl, 2048)!,
                Author = Truncate(fetchedItem.Author, 250),
                PublishedAtUtc = fetchedItem.PublishedAtUtc,
                RetrievedAtUtc = DateTimeOffset.UtcNow,
                ContentHash = CreateSha256Hash(
                    $"{fetchedItem.Title}|{fetchedItem.OriginalUrl}|{fetchedItem.PublishedAtUtc:O}"),
                Category = source.Category
            };

            dbContext.NewsItems.Add(newsItem);

            existingExternalIds.Add(fetchedItem.ExternalId);
            existingOriginalUrls.Add(fetchedItem.OriginalUrl);

            importedItemsCount++;
        }

        source.LastFetchedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Imported {ImportedItemsCount} and skipped {SkippedItemsCount} item(s) from source {SourceName} ({SourceId})",
            importedItemsCount,
            skippedItemsCount,
            source.Name,
            source.Id);

        return new FeedImportResult(
            source.Id,
            source.Name,
            fetchedItems.Count,
            importedItemsCount,
            skippedItemsCount,
            DateTimeOffset.UtcNow);
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