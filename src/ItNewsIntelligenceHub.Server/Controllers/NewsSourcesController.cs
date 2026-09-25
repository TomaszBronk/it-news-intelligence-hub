using ItNewsIntelligenceHub.Server.Contracts.NewsSources;
using ItNewsIntelligenceHub.Server.Domain.Entities;
using ItNewsIntelligenceHub.Server.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ItNewsIntelligenceHub.Server.Application.Feeds;

namespace ItNewsIntelligenceHub.Server.Controllers;

[ApiController]
[Route("api/news-sources")]
public class NewsSourcesController(NewsHubDbContext dbContext, INewsFeedImportService newsFeedImportService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<NewsSourceResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<NewsSourceResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var sources = await dbContext.NewsSources
            .AsNoTracking()
            .OrderBy(source => source.Name)
            .Select(source => new NewsSourceResponse(
                source.Id,
                source.Name,
                source.FeedUrl,
                source.WebsiteUrl,
                source.Category,
                source.IsActive,
                source.CreatedAtUtc,
                source.LastFetchedAtUtc))
            .ToListAsync(cancellationToken);

        return Ok(sources);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(NewsSourceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NewsSourceResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var source = await dbContext.NewsSources
            .AsNoTracking()
            .Where(item => item.Id == id)
            .Select(item => new NewsSourceResponse(
                item.Id,
                item.Name,
                item.FeedUrl,
                item.WebsiteUrl,
                item.Category,
                item.IsActive,
                item.CreatedAtUtc,
                item.LastFetchedAtUtc))
            .SingleOrDefaultAsync(cancellationToken);

        return source is null ? NotFound() : Ok(source);
    }

    [HttpPost]
    [ProducesResponseType(typeof(NewsSourceResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<NewsSourceResponse>> Create(
        CreateNewsSourceRequest request,
        CancellationToken cancellationToken)
    {
        var normalizedFeedUrl = request.FeedUrl.Trim();

        var exists = await dbContext.NewsSources
            .AnyAsync(source => source.FeedUrl == normalizedFeedUrl, cancellationToken);

        if (exists)
        {
            return Conflict(new
            {
                message = "A news source with the same FeedUrl already exists."
            });
        }

        var source = new NewsSource
        {
            Name = request.Name.Trim(),
            FeedUrl = normalizedFeedUrl,
            WebsiteUrl = request.WebsiteUrl?.Trim(),
            Category = request.Category.Trim(),
            IsActive = request.IsActive
        };

        dbContext.NewsSources.Add(source);
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = ToResponse(source);

        return CreatedAtAction(nameof(GetById), new { id = source.Id }, response);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(NewsSourceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<NewsSourceResponse>> Update(
        Guid id,
        UpdateNewsSourceRequest request,
        CancellationToken cancellationToken)
    {
        var source = await dbContext.NewsSources
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);

        if (source is null)
        {
            return NotFound();
        }

        var normalizedFeedUrl = request.FeedUrl.Trim();

        var feedUrlUsedByAnotherSource = await dbContext.NewsSources
            .AnyAsync(
                item => item.Id != id && item.FeedUrl == normalizedFeedUrl,
                cancellationToken);

        if (feedUrlUsedByAnotherSource)
        {
            return Conflict(new
            {
                message = "A news source with the same FeedUrl already exists."
            });
        }

        source.Name = request.Name.Trim();
        source.FeedUrl = normalizedFeedUrl;
        source.WebsiteUrl = request.WebsiteUrl?.Trim();
        source.Category = request.Category.Trim();
        source.IsActive = request.IsActive;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(ToResponse(source));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var source = await dbContext.NewsSources
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);

        if (source is null)
        {
            return NotFound();
        }

        dbContext.NewsSources.Remove(source);
        await dbContext.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpPost("{id:guid}/fetch")]
    [ProducesResponseType(typeof(FeedImportResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<FeedImportResult>> FetchNow(
    Guid id,
    CancellationToken cancellationToken)
    {
        try
        {
            var result = await newsFeedImportService.ImportAsync(id, cancellationToken);

            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
        catch (HttpRequestException exception)
        {
            return StatusCode(
                StatusCodes.Status502BadGateway,
                new
                {
                    message = "The RSS/Atom feed could not be downloaded.",
                    detail = exception.Message
                });
        }
        catch (System.Xml.XmlException exception)
        {
            return StatusCode(
                StatusCodes.Status502BadGateway,
                new
                {
                    message = "The RSS/Atom feed contains invalid XML.",
                    detail = exception.Message
                });
        }
    }

    private static NewsSourceResponse ToResponse(NewsSource source) =>
        new(
            source.Id,
            source.Name,
            source.FeedUrl,
            source.WebsiteUrl,
            source.Category,
            source.IsActive,
            source.CreatedAtUtc,
            source.LastFetchedAtUtc);
}