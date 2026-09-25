using ItNewsIntelligenceHub.Domain.Entities;

namespace ItNewsIntelligenceHub.Application.Abstractions.Persistence;

public interface INewsItemRepository
{
    Task<IReadOnlyList<NewsItem>> GetBySourceIdAsync(
        Guid sourceId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<NewsItem>> GetLatestAsync(
        Guid? sourceId,
        string? category,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task AddRangeAsync(
        IEnumerable<NewsItem> newsItems,
        CancellationToken cancellationToken);
}