using ItNewsIntelligenceHub.Application.Abstractions;
using ItNewsIntelligenceHub.Domain.Entities;
using ItNewsIntelligenceHub.Infrastructure.Persistence;
using ItNewsIntelligenceHub.Server.Contracts.NewsItems;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ItNewsIntelligenceHub.Server.Controllers;

[ApiController]
[Route("api/news-items/{newsItemId:guid}/draft")]
public class PostDraftsController(
    NewsHubDbContext dbContext,
    IPostDraftGenerator draftGenerator)
    : ControllerBase
{
    /// <summary>
    /// Generates a new post draft for the specified news item.
    /// </summary>
    [HttpPost("generate-post")]
    [ProducesResponseType(typeof(PostDraftResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PostDraftResponse>> GeneratePostDraft(
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

        var latestSummary = await dbContext.NewsSummaries
            .Where(s => s.NewsItemId == newsItemId)
            .OrderByDescending(s => s.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

        var draft = await draftGenerator.GeneratePostDraftAsync(
            item,
            latestSummary,
            cancellationToken);

        dbContext.PostDrafts.Add(draft);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(ToResponse(draft));
    }

    /// <summary>
    /// Generates a new discussion prompt for the specified news item.
    /// </summary>
    [HttpPost("generate-discussion")]
    [ProducesResponseType(typeof(PostDraftResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PostDraftResponse>> GenerateDiscussionPrompt(
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

        var latestSummary = await dbContext.NewsSummaries
            .Where(s => s.NewsItemId == newsItemId)
            .OrderByDescending(s => s.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

        var draft = await draftGenerator.GenerateDiscussionPromptAsync(
            item,
            latestSummary,
            cancellationToken);

        dbContext.PostDrafts.Add(draft);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(ToResponse(draft));
    }

    /// <summary>
    /// Gets the latest draft (post or discussion) for the specified news item.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PostDraftResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PostDraftResponse>> GetLatestDraft(
        Guid newsItemId,
        CancellationToken cancellationToken)
    {
        var latest = await dbContext.PostDrafts
            .Where(d => d.NewsItemId == newsItemId)
            .OrderByDescending(d => d.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

        if (latest is null)
        {
            return NotFound();
        }

        return Ok(ToResponse(latest));
    }

    /// <summary>
    /// Updates an existing draft (title, content, isPublished).
    /// </summary>
    [HttpPatch("{draftId:guid}")]
    [ProducesResponseType(typeof(PostDraftResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PostDraftResponse>> UpdateDraft(
        Guid draftId,
        UpdatePostDraftRequest request,
        CancellationToken cancellationToken)
    {
        var draft = await dbContext.PostDrafts
            .SingleOrDefaultAsync(d => d.Id == draftId, cancellationToken);

        if (draft is null)
        {
            return NotFound();
        }

        if (!string.IsNullOrWhiteSpace(request.Title))
        {
            draft.Title = request.Title;
        }

        if (request.Content is not null)
        {
            draft.Content = request.Content;
        }

        if (request.IsPublished.HasValue)
        {
            draft.IsPublished = request.IsPublished.Value;
        }

        draft.UpdatedAtUtc = DateTimeOffset.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(ToResponse(draft));
    }

    private static PostDraftResponse ToResponse(PostDraft draft)
        => new(
            draft.Id,
            draft.NewsItemId,
            draft.Title,
            draft.Content,
            draft.Type,
            draft.SourceUrl,
            draft.IsPublished,
            draft.CreatedAtUtc,
            draft.UpdatedAtUtc);
}