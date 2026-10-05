using System.ComponentModel.DataAnnotations;

namespace ItNewsIntelligenceHub.Server.Contracts.NewsItems;

public class UpdateNewsItemNoteRequest
{
    [StringLength(4000)]
    public string? Note { get; init; }
}
