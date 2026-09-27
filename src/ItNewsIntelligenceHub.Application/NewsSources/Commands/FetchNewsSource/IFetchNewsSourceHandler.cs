namespace ItNewsIntelligenceHub.Application.NewsSources.Commands.FetchNewsSource;

public interface IFetchNewsSourceHandler
{
    Task<FetchNewsSourceResult> HandleAsync(
        FetchNewsSourceCommand command,
        CancellationToken cancellationToken);
}