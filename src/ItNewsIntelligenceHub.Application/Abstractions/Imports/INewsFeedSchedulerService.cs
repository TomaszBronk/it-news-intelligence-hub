namespace ItNewsIntelligenceHub.Application.Abstractions.Imports
{
    public interface INewsFeedSchedulerService
    {
        Task ImportActiveSourcesAsync(CancellationToken cancellationToken);
    }
}
