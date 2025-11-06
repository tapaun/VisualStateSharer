using System.Net.Http.Headers;
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
        imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");
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
            var response = await PostMultipartAsync<ImageInfo>("upload", content);
            Logger.Info($"Image uploaded successfully: {response.PermalinkUrl}");
            return response;
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