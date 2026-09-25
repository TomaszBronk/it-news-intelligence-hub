using ItNewsIntelligenceHub.Application.Abstractions.Persistence;
using ItNewsIntelligenceHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ItNewsIntelligenceHub.Infrastructure.Persistence.Repositories;

public sealed class NewsSourceRepository(NewsHubDbContext dbContext)
    : INewsSourceRepository
{
    public async Task<NewsSource?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await dbContext.NewsSources
            .SingleOrDefaultAsync(source => source.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<NewsSource>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await dbContext.NewsSources
            .AsNoTracking()
            .OrderBy(source => source.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        NewsSource source,
        CancellationToken cancellationToken)
    {
        await dbContext.NewsSources.AddAsync(source, cancellationToken);
    }

    public void Remove(NewsSource source)
    {
        dbContext.NewsSources.Remove(source);
    }

    public Task<bool> FeedUrlExistsAsync(
        string feedUrl,
        Guid? excludedSourceId,
        CancellationToken cancellationToken)
    {
        return dbContext.NewsSources.AnyAsync(
            source => source.FeedUrl == feedUrl
                && (!excludedSourceId.HasValue
                    || source.Id != excludedSourceId.Value),
            cancellationToken);
    }
}