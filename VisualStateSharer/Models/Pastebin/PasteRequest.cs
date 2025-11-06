namespace VisualStateSharer.Models.Pastebin;

public record PasteRequest
{
    public required string Content { get; init; } = string.Empty;
    public required PastePrivacy Privacy { get; init; } = PastePrivacy.Unlisted;
    public required string Expiration { get; init; } = "1H"; // N = Never, 10M, 1H, 1D, 1W, 2W, 1M, 6M, 1Y
    public required string Title { get; init; } = string.Empty;
    public required string Format { get; init; } = "csharp"; // Syntax highlighting
}