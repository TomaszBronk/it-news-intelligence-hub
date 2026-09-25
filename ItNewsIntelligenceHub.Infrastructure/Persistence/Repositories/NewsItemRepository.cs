using ItNewsIntelligenceHub.Application.Abstractions.Persistence;
using ItNewsIntelligenceHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ItNewsIntelligenceHub.Infrastructure.Persistence.Repositories;

public sealed class NewsItemRepository(NewsHubDbContext dbContext)
    : INewsItemRepository
{
    public async Task<IReadOnlyList<NewsItem>> GetBySourceIdAsync(
        Guid sourceId,
        CancellationToken cancellationToken)
    {
        return await dbContext.NewsItems
            .AsNoTracking()
            .Where(item => item.SourceId == sourceId)
            .Select(item => new NewsItem
            {
                Id = item.Id,
                SourceId = item.SourceId,
                ExternalId = item.ExternalId,
                OriginalUrl = item.OriginalUrl
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<NewsItem>> GetLatestAsync(
        Guid? sourceId,
        string? category,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = dbContext.NewsItems
            .AsNoTracking()
            .Include(item => item.Source)
            .AsQueryable();

        if (sourceId.HasValue)
        {
            query = query.Where(item => item.SourceId == sourceId.Value);
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(item => item.Category == category.Trim());
        }

        return await query
            .OrderByDescending(item => item.PublishedAtUtc)
            .ThenByDescending(item => item.RetrievedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task AddRangeAsync(
        IEnumerable<NewsItem> newsItems,
        CancellationToken cancellationToken)
    {
        await dbContext.NewsItems.AddRangeAsync(newsItems, cancellationToken);
    }
}