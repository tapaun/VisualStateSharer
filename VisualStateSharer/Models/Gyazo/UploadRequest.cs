namespace VisualStateSharer.Models.Gyazo;

/// <summary>
/// Request for uploading an image to Gyazo
/// </summary>
public record UploadRequest
{ 
    public required string ImagePath { get; init; }
    public string? Title { get; init; }
    public string? Description { get; init; } = "Uploaded via VisualStateSharer";
    public long Timestamp { get; init; }
}
