using ItNewsIntelligenceHub.Domain.Entities;

namespace ItNewsIntelligenceHub.Application.Abstractions.Summary
{
    public interface ISummaryGenerator
    {
        Task<NewsSummary> GenerateSummaryAsync(
            NewsItem item,
            CancellationToken cancellationToken = default);
    }
}
