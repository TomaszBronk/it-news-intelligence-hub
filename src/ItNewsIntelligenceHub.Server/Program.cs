using ItNewsIntelligenceHub.Application;
using ItNewsIntelligenceHub.Application.Common;
using ItNewsIntelligenceHub.Infrastructure;
using ItNewsIntelligenceHub.Server.BackgroundServices;
using ItNewsIntelligenceHub.Server.Configuration;
using ItNewsIntelligenceHub.Server.ExceptionHandling;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("NewsHubDatabase")
    ?? throw new InvalidOperationException(
        "Connection string 'NewsHubDatabase' was not found.");

builder.Services
    .AddOptions<FeedImportOptions>()
    .Bind(builder.Configuration.GetSection(FeedImportOptions.SectionName))
    .Validate(
        options => options.IntervalMinutes >= 1,
        "FeedImport:IntervalMinutes must be at least 1.")
    .ValidateOnStart();

builder.Services.AddHostedService<NewsFeedImportBackgroundService>();

builder.Services.AddHealthChecks();

builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<ApiExceptionHandler>();

builder.Services.Configure<AiOptions>(
    builder.Configuration.GetSection(AiOptions.SectionName));


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

app.MapHealthChecks("/health");
app.UseExceptionHandler();

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
        path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase) ||
        path.StartsWith("/health", StringComparison.OrdinalIgnoreCase))
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

public partial class Program
{
}