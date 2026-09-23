using System.ComponentModel.DataAnnotations;

namespace ItNewsIntelligenceHub.Server.Contracts.NewsSources;

public class UpdateNewsSourceRequest
{
    [Required]
    [StringLength(150)]
    public string Name { get; init; } = string.Empty;

    [Required]
    [Url]
    [StringLength(2048)]
    public string FeedUrl { get; init; } = string.Empty;

    [Url]
    [StringLength(2048)]
    public string? WebsiteUrl { get; init; }

    [Required]
    [StringLength(50)]
    public string Category { get; init; } = "Other";

    public bool IsActive { get; init; } = true;
}
