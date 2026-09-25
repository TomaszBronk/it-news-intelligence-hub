using ItNewsIntelligenceHub.Domain.Entities;

namespace ItNewsIntelligenceHub.Application.Abstractions.Persistence;

public interface INewsSourceRepository
{
    Task<NewsSource?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<NewsSource>> GetAllAsync(
        CancellationToken cancellationToken);

    Task AddAsync(
        NewsSource source,
        CancellationToken cancellationToken);

    void Remove(NewsSource source);

    Task<bool> FeedUrlExistsAsync(
        string feedUrl,
        Guid? excludedSourceId,
        CancellationToken cancellationToken);
}