namespace ItNewsIntelligenceHub.Server.Contracts.NewsItems;

public record PostDraftResponse(
    Guid Id,
    Guid NewsItemId,
    string Title,
    string Content,
    string Type,
    string SourceUrl,
    bool IsPublished,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? UpdatedAtUtc);