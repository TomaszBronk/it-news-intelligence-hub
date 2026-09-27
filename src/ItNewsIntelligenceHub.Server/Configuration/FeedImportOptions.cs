namespace ItNewsIntelligenceHub.Server.Configuration;

public sealed class FeedImportOptions
{
    public const string SectionName = "FeedImport";

    public int IntervalMinutes { get; init; } = 60;

    public bool RunOnStartup { get; init; } = false;
}