using ItNewsIntelligenceHub.Application.Abstractions;
using ItNewsIntelligenceHub.Application.Abstractions.Summary;
using ItNewsIntelligenceHub.Domain.Entities;
using ItNewsIntelligenceHub.Infrastructure.Persistence;
using ItNewsIntelligenceHub.Server.Contracts.NewsItems;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ItNewsIntelligenceHub.Server.Controllers;

[ApiController]
[Route("api/news-items/{newsItemId:guid}/summary")]
public class NewsSummariesController(
    NewsHubDbContext dbContext,
    ISummaryGenerator summaryGenerator)
    : ControllerBase
{
    /// <summary>
    /// Generates a new Polish AI summary for the specified news item.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(NewsSummaryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NewsSummaryResponse>> GenerateSummary(
        Guid newsItemId,
        CancellationToken cancellationToken)
    {
        var item = await dbContext.NewsItems
            .Include(n => n.Source)
            .SingleOrDefaultAsync(n => n.Id == newsItemId, cancellationToken);

        if (item is null)
        {
            return NotFound();
        }

        var summary = await summaryGenerator.GenerateSummaryAsync(
            item,
            cancellationToken);

        dbContext.NewsSummaries.Add(summary);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(new NewsSummaryResponse(
            summary.Id,
            summary.NewsItemId,
            summary.Content,
            summary.Model,
            summary.CreatedAtUtc));
    }

    /// <summary>
    /// Gets the latest AI summary for the specified news item.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(NewsSummaryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NewsSummaryResponse>> GetLatestSummary(
        Guid newsItemId,
        CancellationToken cancellationToken)
    {
        var latest = await dbContext.NewsSummaries
            .Where(s => s.NewsItemId == newsItemId)
            .OrderByDescending(s => s.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

        if (latest is null)
        {
            return NotFound();
        }

        return Ok(new NewsSummaryResponse(
            latest.Id,
            latest.NewsItemId,
            latest.Content,
            latest.Model,
            latest.CreatedAtUtc));
    }
}