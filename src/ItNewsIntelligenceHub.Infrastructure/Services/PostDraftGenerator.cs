using ItNewsIntelligenceHub.Application.Abstractions;
using ItNewsIntelligenceHub.Application.Common;
using ItNewsIntelligenceHub.Domain.Entities;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

namespace ItNewsIntelligenceHub.Infrastructure.Services;

public sealed class PostDraftGenerator : IPostDraftGenerator
{
    private readonly ChatClient _chatClient;
    private readonly string _model;

    public PostDraftGenerator(IOptions<AiOptions> options)
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

    public async Task<PostDraft> GeneratePostDraftAsync(
        NewsItem item,
        NewsSummary? summary,
        CancellationToken cancellationToken = default)
    {
        var prompt = $"""
            You are a helpful assistant that creates short, developer-focused posts in Polish.
            Based on the IT news and optional AI summary, generate a post draft suitable for LinkedIn or a tech blog.
            Requirements:
            - Language: Polish.
            - Length: 4–8 sentences.
            - Tone: professional, concise, informative.
            - Include:
              - clear opening sentence about what changed,
              - 2–4 key points or implications for developers,
              - one sentence with a practical takeaway or recommendation,
              - Add topical hashtags before the URL,
              - the original source URL at the end.
            - Do not invent facts. Use only the provided information.

            News item:
            Title: {item.Title}
            Category: {item.Category}
            Author: {(string.IsNullOrWhiteSpace(item.Author) ? "Unknown" : item.Author)}
            Published: {item.PublishedAtUtc:yyyy-MM-dd}
            Summary: {(string.IsNullOrWhiteSpace(item.Summary) ? "No summary provided." : item.Summary)}
            URL: {item.OriginalUrl}

            AI summary (optional):
            {(summary?.Content ?? "No AI summary available.")}
            """;

        var response = await _chatClient.CompleteChatAsync(
            [new UserChatMessage(prompt)],
            cancellationToken: cancellationToken);

        var content = response.Value.Content[0].Text;

        var draft = new PostDraft
        {
            Id = Guid.NewGuid(),
            NewsItemId = item.Id,
            Title = $"Post: {item.Title}",
            Content = content,
            Type = "Post",
            SourceUrl = item.OriginalUrl,
            IsPublished = false,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = null
        };

        return draft;
    }

    public async Task<PostDraft> GenerateDiscussionPromptAsync(
        NewsItem item,
        NewsSummary? summary,
        CancellationToken cancellationToken = default)
    {
        var prompt = $"""
            You are a helpful assistant that creates discussion prompts in Polish for developer communities.
            Based on the IT news and optional AI summary, generate 3–5 thoughtful questions that encourage discussion.
            Requirements:
            - Language: Polish.
            - 3–5 questions, each 1–2 sentences.
            - Focus on:
              - practical impact on developers,
              - migration / adoption considerations,
              - pros/cons, trade-offs,
              - real-world usage scenarios.
            - Include the original source URL at the end.
            - Do not invent facts. Use only the provided information.

            News item:
            Title: {item.Title}
            Category: {item.Category}
            Author: {(string.IsNullOrWhiteSpace(item.Author) ? "Unknown" : item.Author)}
            Published: {item.PublishedAtUtc:yyyy-MM-dd}
            Summary: {(string.IsNullOrWhiteSpace(item.Summary) ? "No summary provided." : item.Summary)}
            URL: {item.OriginalUrl}

            AI summary (optional):
            {(summary?.Content ?? "No AI summary available.")}
            """;

        var response = await _chatClient.CompleteChatAsync(
            [new UserChatMessage(prompt)],
            cancellationToken: cancellationToken);

        var content = response.Value.Content[0].Text;

        var draft = new PostDraft
        {
            Id = Guid.NewGuid(),
            NewsItemId = item.Id,
            Title = $"Dyskusja: {item.Title}",
            Content = content,
            Type = "DiscussionPrompt",
            SourceUrl = item.OriginalUrl,
            IsPublished = false,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = null
        };

        return draft;
    }
}