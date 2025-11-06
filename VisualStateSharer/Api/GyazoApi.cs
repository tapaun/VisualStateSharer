using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using VisualStateSharer.Core;
using VisualStateSharer.Models.Gyazo;
using VisualStateSharer.Utils;

namespace VisualStateSharer.Api;

public class GyazoApi(string baseUrl, string accessToken) : ApiClient(baseUrl, accessToken) {
    public async Task<ImageInfo> UploadImageAsync(UploadRequest request)
    {
        Logger.Info($"Uploading image: {request.ImagePath}");
        
        if (!File.Exists(request.ImagePath))
        {
            Logger.Error($"Image file not found: {request.ImagePath}");
            throw new FileNotFoundException($"Image file not found: {request.ImagePath}");
        }

        using var content = new MultipartFormDataContent();
        
        // Add access token
        content.Add(new StringContent(ApiKey), "access_token");
        
        // Add image file
        var imageBytes = await File.ReadAllBytesAsync(request.ImagePath);
        var imageContent = new ByteArrayContent(imageBytes);
        
        // Detect image type from file extension
        var extension = Path.GetExtension(request.ImagePath).ToLower();
        var imageExtension = extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            _ => "image/png"
        };
        
        imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse(imageExtension);
        content.Add(imageContent, "imagedata", Path.GetFileName(request.ImagePath));
        
        // Add optional title
        if (!string.IsNullOrWhiteSpace(request.Title))
        {
            content.Add(new StringContent(request.Title), "title");
        }

        // Add optional description
        if (!string.IsNullOrWhiteSpace(request.Description))
        {
            content.Add(new StringContent(request.Description), "desc");
        }

        try
        {
            var fullUrl = GetFullUrl("upload");
            Logger.Debug($"Posting to: {fullUrl}");
            
            using var client = new HttpClient();
            var response = await client.PostAsync(fullUrl, content);
            var responseBody = await response.Content.ReadAsStringAsync();
            
            Logger.Debug($"Response status: {response.StatusCode}");
            Logger.Debug($"Response body: {responseBody}");
            
            if (!response.IsSuccessStatusCode)
            {
                Logger.Error($"Gyazo API error ({response.StatusCode}): {responseBody}");
                throw new ApiException($"Gyazo API error ({response.StatusCode}): {responseBody}");
            }
            
            JsonSerializerOptions options = new() {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters = { new JsonStringEnumConverter() }
            };
            var imageInfo = JsonSerializer.Deserialize<ImageInfo>(responseBody, options);
            
            if (imageInfo.Equals(default(ImageInfo)))
            {
                throw new ApiException($"Failed to deserialize Gyazo response: {responseBody}");
            }
            
            Logger.Info($"Image uploaded successfully: {imageInfo.PermalinkUrl}");
            return imageInfo;
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to upload image: {request.ImagePath}", ex);
            throw;
        }
    }

    public async Task<ImageInfo> ShareScreenshotAsync(string screenshotPath)
    {
        if (!File.Exists(screenshotPath))
        {
            throw new FileNotFoundException($"Screenshot not found: {screenshotPath}");
        }

        var request = new UploadRequest
        {
            ImagePath = screenshotPath,
            Title = $"Screenshot - {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

        return await UploadImageAsync(request);
    }
}