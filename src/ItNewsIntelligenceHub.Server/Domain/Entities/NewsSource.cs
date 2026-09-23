namespace ItNewsIntelligenceHub.Server.Domain.Entities
{
    public class NewsSource
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = string.Empty;

        public string FeedUrl { get; set; } = string.Empty;

        public string? WebsiteUrl { get; set; }

        public string Category { get; set; } = "Other";

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public DateTime? LastFetchedAtUtc { get; set; }
    }
}
