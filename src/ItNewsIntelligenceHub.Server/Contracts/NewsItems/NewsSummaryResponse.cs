namespace ItNewsIntelligenceHub.Server.Contracts.NewsItems;

public record NewsSummaryResponse(
    Guid Id,
    Guid NewsItemId,
    string Content,
    string Model,
    DateTimeOffset CreatedAtUtc);