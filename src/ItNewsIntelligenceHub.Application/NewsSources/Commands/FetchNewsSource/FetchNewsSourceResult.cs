namespace ItNewsIntelligenceHub.Application.NewsSources.Commands.FetchNewsSource;

public record FetchNewsSourceResult(
    Guid SourceId,
    string SourceName,
    int TotalItemsRead,
    int ImportedItemsCount,
    int SkippedItemsCount,
    DateTimeOffset ImportedAtUtc);