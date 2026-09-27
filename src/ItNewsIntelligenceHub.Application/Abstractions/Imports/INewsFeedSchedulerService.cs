using System;
using System.Collections.Generic;
using System.Text;

namespace ItNewsIntelligenceHub.Application.Abstractions.Imports
{
    public interface INewsFeedSchedulerService
    {
        Task ImportActiveSourcesAsync(CancellationToken cancellationToken);
    }
}
