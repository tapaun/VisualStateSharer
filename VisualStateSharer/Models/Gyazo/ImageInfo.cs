using System.Text.Json.Serialization;

namespace VisualStateSharer.Models.Gyazo;

/// <summary>
/// Response from the Gyazo API after uploading an image
/// </summary>
public record ImageInfo
{
    [JsonPropertyName("image_id")]
    public required string ImageId { get; init; }
    
    [JsonPropertyName("permalink_url")]
    public required string PermalinkUrl { get; init; }
    
    [JsonPropertyName("thumb_url")]
    public required string ThumbUrl { get; init; }
    
    [JsonPropertyName("url")]
    public required string Url { get; init; }
    
    [JsonPropertyName("type")]
    public required string Type { get; init; }
    
    [JsonPropertyName("created_at")]
    public required string CreatedAt { get; init; }
}
