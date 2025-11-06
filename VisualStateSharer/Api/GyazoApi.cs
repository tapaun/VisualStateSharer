using Microsoft.Extensions.Logging;
using Refit;
using VisualStateSharer.Core;
using VisualStateSharer.Interfaces;
using VisualStateSharer.Models.Gyazo;

namespace VisualStateSharer.Api;

public class GyazoApi
{
    private readonly IGyazoApi _api;
    private readonly string _accessToken;
    private readonly ILogger<GyazoApi> _logger;

    public GyazoApi(IGyazoApi api, string accessToken, ILogger<GyazoApi> logger)
    {
        _api = api;
        _accessToken = accessToken;
        _logger = logger;
    }

    public async Task<ImageInfo> UploadImageAsync(UploadRequest request)
    {
        _logger.LogDebug($"Uploading image: {request.ImagePath}");
        
        if (!File.Exists(request.ImagePath))
        {
            _logger.LogError($"Image file not found: {request.ImagePath}");
            throw new FileNotFoundException($"Image file not found: {request.ImagePath}");
        }

        try
        {
            // Read image file
            var imageStream = File.OpenRead(request.ImagePath);
            var fileName = Path.GetFileName(request.ImagePath);
            
            // Detect content type from file extension
            var extension = Path.GetExtension(request.ImagePath).ToLower();
            var contentType = extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "image/png"
            };
            
            var streamPart = new StreamPart(imageStream, fileName, contentType);
            
            _logger.LogDebug($"Posting to Gyazo API");
            
            var imageInfo = await _api.RefitUploadImageAsync(
                _accessToken,
                streamPart,
                request.Title,
                request.Description
            );
            
            _logger.LogInformation($"Image uploaded successfully: {imageInfo.PermalinkUrl}");
            return imageInfo;
        }
        catch (Core.ApiException ex)
        {
            _logger.LogError(ex, $"Failed to upload image: {request.ImagePath}");
            throw new Core.ApiException($"Gyazo API error: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to upload image: {request.ImagePath}");
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
            Title = $"Screenshot - {DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss}",
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

        return await UploadImageAsync(request);
    }
}