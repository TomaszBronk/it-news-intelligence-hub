using ItNewsIntelligenceHub.Application.Abstractions.Imports;
using ItNewsIntelligenceHub.Server.Configuration;
using Microsoft.Extensions.Options;

namespace ItNewsIntelligenceHub.Server.BackgroundServices;

public sealed class NewsFeedImportBackgroundService(
    IServiceScopeFactory serviceScopeFactory,
    IOptions<FeedImportOptions> options,
    ILogger<NewsFeedImportBackgroundService> logger)
    : BackgroundService
{
    private readonly FeedImportOptions _options = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        ValidateOptions(_options);

        logger.LogInformation(
            "News feed import background service started. Interval: {IntervalMinutes} minute(s). Run on startup: {RunOnStartup}.",
            _options.IntervalMinutes,
            _options.RunOnStartup);

        if (_options.RunOnStartup)
        {
            await RunImportCycleAsync(stoppingToken);
        }

        using var timer = new PeriodicTimer(
            TimeSpan.FromMinutes(_options.IntervalMinutes));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await RunImportCycleAsync(stoppingToken);
        }
    }

    private async Task RunImportCycleAsync(CancellationToken stoppingToken)
    {
        try
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();

            var scheduler = scope.ServiceProvider
                .GetRequiredService<INewsFeedSchedulerService>();

            logger.LogInformation("Starting scheduled news feed import cycle.");

            await scheduler.ImportActiveSourcesAsync(stoppingToken);

            logger.LogInformation("Scheduled news feed import cycle completed.");
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Scheduled news feed import cycle was cancelled.");
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Scheduled news feed import cycle failed.");
        }
    }
    private static void ValidateOptions(FeedImportOptions options)
    {
        if (options.IntervalMinutes < 1)
        {
            throw new InvalidOperationException(
                "FeedImport:IntervalMinutes must be at least 1.");
        }
    }
}