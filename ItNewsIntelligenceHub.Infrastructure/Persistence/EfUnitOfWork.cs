using ItNewsIntelligenceHub.Application.Abstractions.Persistence;

namespace ItNewsIntelligenceHub.Infrastructure.Persistence;

public sealed class EfUnitOfWork(NewsHubDbContext dbContext) : IUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}