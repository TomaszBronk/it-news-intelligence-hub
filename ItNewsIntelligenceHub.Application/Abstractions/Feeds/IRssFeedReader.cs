using ItNewsIntelligenceHub.Application.NewsSources.Commands.FetchNewsSource;

namespace ItNewsIntelligenceHub.Application.Abstractions.Feeds
{
    public interface IRssFeedReader
    {
        Task<IReadOnlyCollection<FeedNewsItem>> ReadAsync(
            Uri feedUrl,
            CancellationToken cancellationToken);
    }
}
