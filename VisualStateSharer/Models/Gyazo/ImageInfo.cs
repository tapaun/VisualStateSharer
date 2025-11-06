using System.Text.Json.Serialization;

namespace VisualStateSharer.Models.Gyazo;

/// <summary>
/// Response from the Gyazo API after uploading an image
/// </summary>
public struct ImageInfo
{
    [JsonPropertyName("image_id")]
    public string ImageId { get; set; }
    
    [JsonPropertyName("permalink_url")]
    public string PermalinkUrl { get; set; }
    
    [JsonPropertyName("thumb_url")]
    public string ThumbUrl { get; set; }
    
    [JsonPropertyName("url")]
    public string Url { get; set; }
    
    [JsonPropertyName("type")]
    public string Type { get; set; }
    
    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; }
}
