using ItNewsIntelligenceHub.Domain.Entities;

namespace ItNewsIntelligenceHub.Application.Abstractions;

public interface IPostDraftGenerator
{
    Task<PostDraft> GeneratePostDraftAsync(
        NewsItem item,
        NewsSummary? summary,
        CancellationToken cancellationToken = default);

    Task<PostDraft> GenerateDiscussionPromptAsync(
        NewsItem item,
        NewsSummary? summary,
        CancellationToken cancellationToken = default);
}