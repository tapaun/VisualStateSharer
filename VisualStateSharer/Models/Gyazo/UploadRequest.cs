namespace VisualStateSharer.Models.Gyazo;

/// <summary>
/// Request for uploading an image to Gyazo
/// </summary>
public struct UploadRequest
{
    public string ImagePath { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public long Timestamp { get; set; }
}
