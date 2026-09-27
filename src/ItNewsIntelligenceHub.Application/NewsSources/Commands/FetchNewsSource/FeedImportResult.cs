namespace ItNewsIntelligenceHub.Application.Feeds
{
    public record FeedImportResult(
       Guid SourceId,
       string SourceName,
       int TotalItemsRead,
       int ImportedItemsCount,
       int SkippedItemsCount,
       DateTimeOffset ImportedAtUtc);
}
