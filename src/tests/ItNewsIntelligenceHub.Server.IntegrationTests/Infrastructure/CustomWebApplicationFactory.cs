using ItNewsIntelligenceHub.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace ItNewsIntelligenceHub.Server.IntegrationTests.Infrastructure;

public sealed class CustomWebApplicationFactory
    : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection =
        new("Data Source=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.UseSetting(
            "ConnectionStrings:NewsHubDatabase",
            "Data Source=integration-tests-placeholder.db");

        builder.ConfigureServices(services =>
        {
            RemoveHostedServices(services);
            RemoveProductionDbContextRegistrations(services);

            if (_connection.State != System.Data.ConnectionState.Open)
            {
                _connection.Open();
            }

            services.AddDbContext<NewsHubDbContext>(options =>
                options.UseSqlite(_connection));

            using var scope = services.BuildServiceProvider()
                .CreateScope();

            var dbContext = scope.ServiceProvider
                .GetRequiredService<NewsHubDbContext>();

            dbContext.Database.EnsureCreated();
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _connection.Dispose();
        }

        base.Dispose(disposing);
    }

    private static void RemoveHostedServices(
        IServiceCollection services)
    {
        var hostedServices = services
            .Where(descriptor =>
                descriptor.ServiceType == typeof(IHostedService))
            .ToList();

        foreach (var hostedService in hostedServices)
        {
            services.Remove(hostedService);
        }
    }

    private static void RemoveProductionDbContextRegistrations(
        IServiceCollection services)
    {
        services.RemoveAll<IDbContextOptionsConfiguration<NewsHubDbContext>>();
        services.RemoveAll<DbContextOptions<NewsHubDbContext>>();
        services.RemoveAll<DbContextOptions>();
        services.RemoveAll<NewsHubDbContext>();
    }
}