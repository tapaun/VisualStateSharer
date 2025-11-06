namespace VisualStateSharer.Configuration;

public record GyazoSettings
{
    public required string BaseUrl { get; init; } = "https://upload.gyazo.com/api";
    public required string AccessToken { get; init; }
    public int Timeout { get; init; } = 30;
}
