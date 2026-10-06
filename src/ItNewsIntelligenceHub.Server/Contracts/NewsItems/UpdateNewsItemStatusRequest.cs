using ItNewsIntelligenceHub.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ItNewsIntelligenceHub.Server.Contracts.NewsItems;

public class UpdateNewsItemStatusRequest
{
    [Required]
    [EnumDataType(typeof(NewsItemStatus))]
    public NewsItemStatus Status { get; init; }
}