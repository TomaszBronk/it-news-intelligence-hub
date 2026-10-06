using System.ComponentModel.DataAnnotations;

namespace ItNewsIntelligenceHub.Server.Contracts.NewsItems;

public class UpdatePostDraftRequest
{
    [StringLength(500)]
    public string? Title { get; init; }

    [StringLength(4000)]
    public string? Content { get; init; }

    public bool? IsPublished { get; init; }
}