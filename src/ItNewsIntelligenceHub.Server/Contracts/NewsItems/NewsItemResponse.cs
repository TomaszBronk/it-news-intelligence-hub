namespace ItNewsIntelligenceHub.Server.Contracts.NewsItems
{
    public record NewsItemResponse(
        Guid Id,
        Guid SourceId,
        string SourceName,
        string Title,
        string? Summary,
        string OriginalUrl,
        string? Author,
        DateTimeOffset? PublishedAtUtc,
        DateTimeOffset RetrievedAtUtc,
        string Category);
}
