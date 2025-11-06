namespace VisualStateSharer.Configuration;

public record PastebinSettings
{
    public required string BaseUrl { get; init; } = "https://pastebin.com/api";
    public required string ApiKey { get; init; }
    public int Timeout { get; init; } = 30;
}
