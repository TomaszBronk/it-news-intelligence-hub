namespace ItNewsIntelligenceHub.Domain.Entities;

public class PostDraft
{
    public Guid Id { get; set; }

    public Guid NewsItemId { get; set; }

    public NewsItem? NewsItem { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string Type { get; set; } = "Post"; // "Post" | "DiscussionPrompt"

    public string SourceUrl { get; set; } = string.Empty;

    public bool IsPublished { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset? UpdatedAtUtc { get; set; }
}