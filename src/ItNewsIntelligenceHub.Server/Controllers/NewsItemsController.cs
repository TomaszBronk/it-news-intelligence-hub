using ItNewsIntelligenceHub.Server.Contracts.NewsItems;
using ItNewsIntelligenceHub.Infrastructure.Persistence;
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
                item.Category))
            .ToListAsync(cancellationToken);

        return Ok(items);
    }
}