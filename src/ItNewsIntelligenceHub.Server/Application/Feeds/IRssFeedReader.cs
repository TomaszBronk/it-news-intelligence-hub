namespace ItNewsIntelligenceHub.Server.Application.Feeds
{
    public interface IRssFeedReader
    {
        Task<IReadOnlyCollection<FeedNewsItem>> ReadAsync(
            Uri feedUrl,
            CancellationToken cancellationToken);
    }
}
