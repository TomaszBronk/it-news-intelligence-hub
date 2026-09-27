using ItNewsIntelligenceHub.Application.Abstractions.Imports;
using ItNewsIntelligenceHub.Application.Abstractions.Persistence;
using ItNewsIntelligenceHub.Application.Abstractions.Time;
using ItNewsIntelligenceHub.Application.NewsSources.Commands.FetchNewsSource;

namespace ItNewsIntelligenceHub.Application.NewsSources.Services;

public sealed class NewsFeedSchedulerService(
    INewsSourceRepository newsSourceRepository,
    IUnitOfWork unitOfWork,
    IFetchNewsSourceHandler fetchNewsSourceHandler,
    IClock clock) : INewsFeedSchedulerService
{
    public async Task ImportActiveSourcesAsync(
        CancellationToken cancellationToken)
    {
        var sources = await newsSourceRepository.GetAllAsync(cancellationToken);

        foreach (var source in sources.Where(source => source.IsActive))
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                source.LastFetchAttemptAtUtc = clock.UtcNow;
                source.LastFetchError = null;

                await unitOfWork.SaveChangesAsync(cancellationToken);

                await fetchNewsSourceHandler.HandleAsync(
                    new FetchNewsSourceCommand(source.Id),
                    cancellationToken);

                source.LastSuccessfulFetchAtUtc = clock.UtcNow;
                source.LastFetchedAtUtc = clock.UtcNow;
                source.LastFetchError = null;

                await unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                source.LastFetchError = Truncate(exception.Message, 2000);

                await unitOfWork.SaveChangesAsync(CancellationToken.None);
            }
        }
    }

    private static string Truncate(string value, int maximumLength)
    {
        return value.Length <= maximumLength
            ? value
            : value[..maximumLength];
    }
}