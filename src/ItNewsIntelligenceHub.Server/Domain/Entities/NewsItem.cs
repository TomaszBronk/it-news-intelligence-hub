namespace ItNewsIntelligenceHub.Server.Domain.Entities
{
    public class NewsItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid SourceId { get; set; }

        public NewsSource Source { get; set; } = null!;

        public string ExternalId { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string? Summary { get; set; }

        public string OriginalUrl { get; set; } = string.Empty;

        public string? Author { get; set; }

        public DateTimeOffset? PublishedAtUtc { get; set; }

        public DateTimeOffset RetrievedAtUtc { get; set; } = DateTimeOffset.UtcNow;

        public string ContentHash { get; set; } = string.Empty;

        public string Category { get; set; } = "Other";
    }
}
