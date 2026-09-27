using System;
using System.Collections.Generic;
using System.Text;

namespace ItNewsIntelligenceHub.Application.Abstractions.Persistence
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
