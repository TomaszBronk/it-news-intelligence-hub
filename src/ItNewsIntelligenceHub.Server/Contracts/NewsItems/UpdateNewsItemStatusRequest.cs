using System.ComponentModel.DataAnnotations;
using ItNewsIntelligenceHub.Domain.Enums;

namespace ItNewsIntelligenceHub.Server.Contracts.NewsItems;

public class UpdateNewsItemStatusRequest
{
    [Required]
    [EnumDataType(typeof(NewsItemStatus))]
    public NewsItemStatus Status { get; init; }
}