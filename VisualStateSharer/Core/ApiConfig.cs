namespace VisualStateSharer.Core;

public class ApiConfig 
{
    // Pastebin Configuration
    public static string PastebinBaseUrl => "https://pastebin.com/api";
    public static string PastebinPostEndpoint => "/api_post.php";
    public static string? PastebinApiKey => Environment.GetEnvironmentVariable("PASTEBIN_API_KEY");
    
    // Gyazo Configuration
    public static string GyazoBaseUrl => "https://upload.gyazo.com/api";
    public static string GyazoUploadEndpoint => "/upload";
    public static string? GyazoClientId => Environment.GetEnvironmentVariable("GYAZO_CLIENT_ID");
    public static string? GyazoClientSecret => Environment.GetEnvironmentVariable("GYAZO_CLIENT_SECRET");
    
    // General Settings
    public static int DefaultTimeoutSeconds => 30;
    public static int MaxRetries => 3;
    public static bool EnableLogging => true;
}