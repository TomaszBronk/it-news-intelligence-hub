namespace ItNewsIntelligenceHub.Domain.Entities;

public class NewsSummary
{
    public Guid Id { get; set; }

    public Guid NewsItemId { get; set; }

    public NewsItem? NewsItem { get; set; }

    public string Content { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public DateTimeOffset CreatedAtUtc { get; set; }
}