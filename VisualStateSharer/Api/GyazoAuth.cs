using System.Text.Json;
using VisualStateSharer.Core;
using VisualStateSharer.Utils;

namespace VisualStateSharer.Api;

/// <summary>
/// Helper class to obtain OAuth access token from Gyazo
/// </summary>
public class GyazoAuth
{
    private readonly string _clientId;
    private readonly string _clientSecret;
    private const string AuthUrl = "https://api.gyazo.com/api/token";

    public GyazoAuth(string clientId, string clientSecret)
    {
        _clientId = clientId;
        _clientSecret = clientSecret;
    }

    /// <summary>
    /// Get an access token using client credentials flow
    /// </summary>
    public async Task<string> GetAccessTokenAsync()
    {
        using var client = new HttpClient();
        
        var requestData = new Dictionary<string, string>
        {
            { "client_id", _clientId },
            { "client_secret", _clientSecret },
            { "grant_type", "client_credentials" },
            { "redirect_uri", "http://localhost" }
        };

        var content = new FormUrlEncodedContent(requestData);
        var response = await client.PostAsync(AuthUrl, content);
        
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new ApiException($"Failed to get access token: {response.StatusCode} - {error}");
        }

        var responseJson = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<JsonElement>(responseJson);
        
        if (tokenResponse.TryGetProperty("access_token", out var accessToken))
        {
            return accessToken.GetString() ?? throw new ApiException("Access token is null");
        }

        throw new ApiException("No access_token in response");
    }
}

