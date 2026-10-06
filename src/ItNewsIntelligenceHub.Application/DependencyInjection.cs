using ItNewsIntelligenceHub.Application.Abstractions.Imports;
using ItNewsIntelligenceHub.Application.NewsSources.Commands.FetchNewsSource;
using ItNewsIntelligenceHub.Application.NewsSources.Services;
using Microsoft.Extensions.DependencyInjection;


namespace ItNewsIntelligenceHub.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IFetchNewsSourceHandler, FetchNewsSourceHandler>();
        services.AddScoped<INewsFeedSchedulerService, NewsFeedSchedulerService>();

        return services;
    }
}