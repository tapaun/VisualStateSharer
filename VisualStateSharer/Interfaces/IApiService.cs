namespace VisualStateSharer.Interfaces;

public interface IApiService
{   
    /// <summary>
    /// Constructs the full URL by combining base URL and endpoint
    /// </summary>
    string GetFullUrl(string endpoint);
    
    /// <summary>
    /// Validates that the API client is properly configured
    /// </summary>
    bool IsConfigured();
}