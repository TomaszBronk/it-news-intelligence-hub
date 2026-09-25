using ItNewsIntelligenceHub.Application.Feeds;
using ItNewsIntelligenceHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using ItNewsIntelligenceHub.Infrastructure.Feeds;
using ItNewsIntelligenceHub.Application.Abstractions.Feeds;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("NewsHubDatabase")
    ?? throw new InvalidOperationException(
        "Connection string 'NewsHubDatabase' was not found.");

builder.Services.AddDbContext<NewsHubDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddHttpClient<IRssFeedReader, RssFeedReader>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddScoped<INewsFeedImportService, NewsFeedImportService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Serve static files and SPA
app.UseDefaultFiles();
app.MapStaticAssets();

app.MapFallback(async context =>
{
    var path = context.Request.Path.Value ?? "";

    // Nie fallback dla API, OpenAPI, Swagger
    if (path.StartsWith("/api", StringComparison.OrdinalIgnoreCase) ||
        path.StartsWith("/openapi", StringComparison.OrdinalIgnoreCase) ||
        path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase))
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        return;
    }

    // Dla wszystkich innych ścieżek - zwróć SPA index.html
    var indexPath = Path.Combine(app.Environment.WebRootPath, "index.html");
    if (System.IO.File.Exists(indexPath))
    {
        context.Response.ContentType = "text/html; charset=utf-8";
        await context.Response.SendFileAsync(indexPath);
    }
});

app.Run();
