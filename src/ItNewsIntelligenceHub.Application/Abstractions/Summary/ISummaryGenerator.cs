using ItNewsIntelligenceHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ItNewsIntelligenceHub.Application.Abstractions.Summary
{
    public interface ISummaryGenerator
    {
        Task<NewsSummary> GenerateSummaryAsync(
            NewsItem item,
            CancellationToken cancellationToken = default);
    }
}
