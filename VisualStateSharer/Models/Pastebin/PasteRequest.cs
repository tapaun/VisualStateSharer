namespace VisualStateSharer.Models.Pastebin;

public record PasteRequest
{
    public string Content { get; init; } = string.Empty;
    public PastePrivacy Privacy { get; init; } = PastePrivacy.Unlisted;
    public string Expiration { get; init; } = "1H"; // N = Never, 10M, 1H, 1D, 1W, 2W, 1M, 6M, 1Y
    public string Title { get; init; } = string.Empty;
    public string Format { get; init; } = "csharp"; // Syntax highlighting
}