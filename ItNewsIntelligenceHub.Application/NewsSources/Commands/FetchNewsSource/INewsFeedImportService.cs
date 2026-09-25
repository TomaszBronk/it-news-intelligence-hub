namespace ItNewsIntelligenceHub.Application.Feeds;

public interface INewsFeedImportService
{
    Task<FeedImportResult> ImportAsync(
        Guid sourceId,
        CancellationToken cancellationToken);
}