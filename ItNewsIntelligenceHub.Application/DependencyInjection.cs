using ItNewsIntelligenceHub.Application.NewsSources.Commands.FetchNewsSource;
using Microsoft.Extensions.DependencyInjection;

namespace ItNewsIntelligenceHub.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IFetchNewsSourceHandler, FetchNewsSourceHandler>();

        return services;
    }
}