namespace ItNewsIntelligenceHub.Application.Common;

public class AiOptions
{
    public const string SectionName = "Ai";

    public string Provider { get; set; } = "OpenAI";

    public OpenAiOptions OpenAi { get; set; } = new();
}

public class OpenAiOptions
{
    public string ApiKey { get; set; } = string.Empty;

    public string Model { get; set; } = "gpt-4o-mini";
}