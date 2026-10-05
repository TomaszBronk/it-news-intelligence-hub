using ItNewsIntelligenceHub.Domain.Enums;
using ItNewsIntelligenceHub.Infrastructure.Persistence;
using ItNewsIntelligenceHub.Server.Contracts.NewsItems;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ItNewsIntelligenceHub.Server.Controllers;

[ApiController]
[Route("api/briefings")]
public class BriefingsController(NewsHubDbContext dbContext)
    : ControllerBase
{
    [HttpGet("saved")]
    [ProducesResponseType(typeof(List<NewsItemResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<NewsItemResponse>>> GetSavedBriefing(
        CancellationToken cancellationToken)
    {
        var items = await dbContext.NewsItems
            .Include(item => item.Source)
            .Where(item => item.Status == NewsItemStatus.Saved)
            .OrderByDescending(item => item.PublishedAtUtc)
            .ThenByDescending(item => item.RetrievedAtUtc)
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
                item.Status,
                item.Note))
            .ToListAsync(cancellationToken);

        return Ok(items);
    }

    [HttpGet("daily")]
    [ProducesResponseType(typeof(List<NewsItemResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<NewsItemResponse>>> GetDailyBriefing(
        CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var yesterday = now.AddDays(-1);

        var items = await dbContext.NewsItems
            .Include(item => item.Source)
            .Where(item =>
                item.PublishedAtUtc >= yesterday &&
                item.PublishedAtUtc <= now)
            .OrderByDescending(item => item.PublishedAtUtc)
            .ThenByDescending(item => item.RetrievedAtUtc)
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
                item.Status,
                item.Note))
            .ToListAsync(cancellationToken);

        return Ok(items);
    }

    [HttpGet("weekly")]
    [ProducesResponseType(typeof(List<NewsItemResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<NewsItemResponse>>> GetWeeklyBriefing(
        CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var weekAgo = now.AddDays(-7);

        var items = await dbContext.NewsItems
            .Include(item => item.Source)
            .Where(item =>
                item.PublishedAtUtc >= weekAgo &&
                item.PublishedAtUtc <= now)
            .OrderByDescending(item => item.PublishedAtUtc)
            .ThenByDescending(item => item.RetrievedAtUtc)
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
                item.Status,
                item.Note))
            .ToListAsync(cancellationToken);

        return Ok(items);
    }
}