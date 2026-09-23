namespace ItNewsIntelligenceHub.Server.Contracts.NewsSources;

public record NewsSourceResponse(
    Guid Id,
    string Name,
    string FeedUrl,
    string? WebsiteUrl,
    string Category,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? LastFetchedAtUtc);