using ItNewsIntelligenceHub.Application.Abstractions.Time;

namespace ItNewsIntelligenceHub.Infrastructure.Time;

public sealed class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;

    public DateTimeOffset UtcNowOffset => DateTimeOffset.UtcNow;
}