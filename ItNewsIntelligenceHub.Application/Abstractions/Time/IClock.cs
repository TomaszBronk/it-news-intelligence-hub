using System;
using System.Collections.Generic;
using System.Text;

namespace ItNewsIntelligenceHub.Application.Abstractions.Time
{
    public interface IClock
    {
        DateTime UtcNow { get; }

        DateTimeOffset UtcNowOffset { get; }
    }
}
