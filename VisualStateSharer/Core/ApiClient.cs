using System.Text;
using VisualStateSharer.Interfaces;
using VisualStateSharer.Utils;

namespace VisualStateSharer.Core;

public abstract class ApiClient(string baseUrl, string apiKey) : IApiService {
    private string BaseUrl { get; } = baseUrl;
    protected string ApiKey { get; } = apiKey;

    public string GetFullUrl(string endpoint) {
        return $"{BaseUrl.TrimEnd('/')}/{endpoint.TrimStart('/')}";
    }
    
    public bool IsConfigured()
    {
        return !string.IsNullOrEmpty(BaseUrl) && !string.IsNullOrEmpty(ApiKey);
    }
}
