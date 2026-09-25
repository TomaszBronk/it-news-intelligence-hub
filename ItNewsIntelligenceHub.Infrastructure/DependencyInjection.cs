using ItNewsIntelligenceHub.Application.Abstractions.Feeds;
using ItNewsIntelligenceHub.Application.Abstractions.Persistence;
using ItNewsIntelligenceHub.Application.Abstractions.Time;
using ItNewsIntelligenceHub.Infrastructure.Feeds;
using ItNewsIntelligenceHub.Infrastructure.Persistence;
using ItNewsIntelligenceHub.Infrastructure.Persistence.Repositories;
using ItNewsIntelligenceHub.Infrastructure.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;

namespace ItNewsIntelligenceHub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("NewsHubDatabase")
            ?? throw new InvalidOperationException(
                "Connection string 'NewsHubDatabase' was not found.");

        services.AddDbContext<NewsHubDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<INewsSourceRepository, NewsSourceRepository>();
        services.AddScoped<INewsItemRepository, NewsItemRepository>();
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();

        services.AddSingleton<IClock, SystemClock>();

        services.AddHttpClient<IRssFeedReader, RssFeedReader>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);

            client.DefaultRequestHeaders.UserAgent.ParseAdd(
                "ITNewsIntelligenceHub/1.0 (+https://github.com/TomaszBronk/it-news-intelligence-hub)");
        });

        return services;
    }
}