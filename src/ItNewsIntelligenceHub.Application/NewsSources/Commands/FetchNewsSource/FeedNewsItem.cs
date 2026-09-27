namespace ItNewsIntelligenceHub.Application.NewsSources.Commands.FetchNewsSource;

public record FeedNewsItem(
    string ExternalId,
    string Title,
    string? Summary,
    string OriginalUrl,
    string? Author,
    DateTimeOffset? PublishedAtUtc);