using System.Text;
using VisualStateSharer.Interfaces;
using VisualStateSharer.Utils;

namespace VisualStateSharer.Core;

public abstract class ApiClient : IApiService {
    private readonly HttpClient _httpClient;
    protected readonly JsonSerializerHelper _serializer;
    private string BaseUrl { get; }
    protected string ApiKey { get; }

    protected ApiClient(string baseUrl, string apiKey) {
        BaseUrl = baseUrl;
        ApiKey = apiKey;
        _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        _serializer = new JsonSerializerHelper();
    }

    public string GetFullUrl(string endpoint) {
        return $"{BaseUrl.TrimEnd('/')}/{endpoint.TrimStart('/')}";
    }
    
    public bool IsConfigured()
    {
        return !string.IsNullOrEmpty(BaseUrl) && !string.IsNullOrEmpty(ApiKey);
    }

    protected async Task<string> PostFormAsync(string endpoint, Dictionary<string, string> formData) {
        var content = new FormUrlEncodedContent(formData);
        var response = await _httpClient.PostAsync(endpoint, content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
    
    protected async Task<TResponse> PostJsonAsync<TResponse>(string endpoint, object data)
    {
        var json = _serializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(endpoint, content);
        response.EnsureSuccessStatusCode();
        
        var responseJson = await response.Content.ReadAsStringAsync();
        return _serializer.Deserialize<TResponse>(responseJson) ?? throw new ApiException("Failed to deserialize response");
    }
    
    protected async Task<TResponse> PostMultipartAsync<TResponse>(string endpoint, MultipartFormDataContent content)
    {
        var response = await _httpClient.PostAsync(endpoint, content);
        response.EnsureSuccessStatusCode();
        
        var responseJson = await response.Content.ReadAsStringAsync();
        return _serializer.Deserialize<TResponse>(responseJson) ?? throw new ApiException("Failed to deserialize response");
    }
    
    protected async Task<string> GetAsync(string endpoint) {
        var response = await _httpClient.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
    
    public void Dispose() => _httpClient?.Dispose();
}
