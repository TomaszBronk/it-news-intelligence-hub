using ItNewsIntelligenceHub.Domain.Enums;
using ItNewsIntelligenceHub.Infrastructure.Persistence;
using ItNewsIntelligenceHub.Server.Contracts.NewsItems;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ItNewsIntelligenceHub.Server.Controllers;

[ApiController]
[Route("api/news-items")]
public class NewsItemsController(NewsHubDbContext dbContext) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<NewsItemResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<NewsItemResponse>>> GetAll(
        [FromQuery] Guid? sourceId,
        [FromQuery] string? category,
        [FromQuery] NewsItemStatus? status,
        [FromQuery] DateTimeOffset? publishedFromUtc,
        [FromQuery] DateTimeOffset? publishedToUtc,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

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
            var normalizedCategory = category.Trim();

            query = query.Where(item => item.Category == normalizedCategory);
        }

        if (status.HasValue)
        {
            query = query.Where(item => item.Status == status.Value);
        }

        if (publishedFromUtc.HasValue)
        {
            query = query.Where(item =>
                item.PublishedAtUtc >= publishedFromUtc.Value);
        }

        if (publishedToUtc.HasValue)
        {
            query = query.Where(item =>
                item.PublishedAtUtc <= publishedToUtc.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim();
            query = query.Where(item => item.Title.Contains(normalizedSearch));
        }

        var items = await query
            .OrderByDescending(item => item.PublishedAtUtc)
            .ThenByDescending(item => item.RetrievedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(item => new NewsItemResponse(
                item.Id,
                item.SourceId,
                item.Source.Name,
                item.Title,
                item.Summary,
                item.OriginalUrl,
                item.Author,
                item.PublishedAtUtc,
                item.RetrievedAtUtc,
                item.Category,
                item.Status
            ))
            .ToListAsync(cancellationToken);

        return Ok(items);
    }

    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(NewsItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NewsItemResponse>> UpdateStatus(
    Guid id,
    UpdateNewsItemStatusRequest request,
    CancellationToken cancellationToken)
    {
        var item = await dbContext.NewsItems
            .Include(newsItem => newsItem.Source)
            .SingleOrDefaultAsync(
                newsItem => newsItem.Id == id,
                cancellationToken);

        if (item is null)
        {
            return NotFound();
        }

        item.Status = request.Status;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(new NewsItemResponse(
            item.Id,
            item.SourceId,
            item.Source.Name,
            item.Title,
            item.Summary,
            item.OriginalUrl,
            item.Author,
            item.PublishedAtUtc,
            item.RetrievedAtUtc,
            item.Category,
            item.Status));
    }
}