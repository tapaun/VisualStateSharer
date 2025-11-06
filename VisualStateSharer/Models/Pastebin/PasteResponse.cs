namespace VisualStateSharer.Models.Pastebin;

public class PasteResponse
{
    public string Url { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
        
    public static PasteResponse FromUrl(string url)
    {
        var key = url.Split('/').Last();
        return new PasteResponse 
        { 
            Url = url, 
            Key = key,
            CreatedAt = DateTime.UtcNow 
        };
    }
}