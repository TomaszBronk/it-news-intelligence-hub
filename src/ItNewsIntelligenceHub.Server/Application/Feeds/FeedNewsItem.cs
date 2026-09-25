namespace ItNewsIntelligenceHub.Server.Application.Feeds
{
    public record FeedNewsItem(
        string ExternalId,
        string Title,
        string? Summary,
        string OriginalUrl,
        string? Author,
        DateTimeOffset? PublishedAtUtc);
}
