using ItNewsIntelligenceHub.Application.Abstractions.Summary;
using ItNewsIntelligenceHub.Application.Common;
using ItNewsIntelligenceHub.Domain.Entities;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

namespace ItNewsIntelligenceHub.Infrastructure.Services;

public sealed class OpenAiSummaryGenerator : ISummaryGenerator
{
    private readonly ChatClient _chatClient;
    private readonly string _model;

    public OpenAiSummaryGenerator(IOptions<AiOptions> options)
    {
        var ai = options.Value;
        var openAi = ai.OpenAi;

        if (string.IsNullOrWhiteSpace(openAi.ApiKey))
        {
            throw new InvalidOperationException(
                "OpenAI API key is not configured.");
        }

        var client = new OpenAIClient(new ApiKeyCredential(openAi.ApiKey));

        _model = openAi.Model;
        _chatClient = client.GetChatClient(_model);
    }

    public async Task<NewsSummary> GenerateSummaryAsync(
        NewsItem item,
        CancellationToken cancellationToken = default)
    {
        var prompt = $"""
            You are a helpful assistant that summarizes IT news in Polish.
            Create a short, factual summary in Polish (6–10 sentences).
            Do not add opinions or speculation.
            Use clear, professional language suitable for developers.

            News item:
            Title: {item.Title}
            Category: {item.Category}
            Author: {(string.IsNullOrWhiteSpace(item.Author) ? "Unknown" : item.Author)}
            Published: {item.PublishedAtUtc:yyyy-MM-dd}
            Summary: {(string.IsNullOrWhiteSpace(item.Summary) ? "No summary provided." : item.Summary)}
            URL: {item.OriginalUrl}
            """;

        var chatMessage = new UserChatMessage(prompt);

        var response = await _chatClient.CompleteChatAsync(
            [chatMessage],
            cancellationToken: cancellationToken);

        var content = response.Value.Content[0].Text;

        var summary = new NewsSummary
        {
            Id = Guid.NewGuid(),
            NewsItemId = item.Id,
            Content = content,
            Model = _model,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        return summary;
    }
}