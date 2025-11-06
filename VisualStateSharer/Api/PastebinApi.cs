using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Refit;
using VisualStateSharer.Core;
using VisualStateSharer.Interfaces;
using VisualStateSharer.Models.Pastebin;

namespace VisualStateSharer.Api;

public class PastebinApi
{
    private readonly IPastebinApi _api;
    private readonly string _apiKey;
    private readonly ILogger<PastebinApi> _logger;

    public PastebinApi(IPastebinApi api, string apiKey, ILogger<PastebinApi> logger)
    {
        _api = api;
        _apiKey = apiKey;
        _logger = logger;
    }

    public async Task<PasteResponse> CreatePasteAsync(PasteRequest request)
    {
        _logger.LogInformation($"Creating paste: {request.Title}");
        
        var formData = new Dictionary<string, string>
        {
            { "api_dev_key", _apiKey },
            { "api_option", "paste" },
            { "api_paste_code", request.Content },
            { "api_paste_private", ((int)request.Privacy).ToString() },
            { "api_paste_expire_date", request.Expiration },
            { "api_paste_name", request.Title },
            { "api_paste_format", request.Format }
        };

        try
        {
            _logger.LogDebug($"Posting to Pastebin API");
            
            var responseUrl = await _api.RefitCreatePasteAsync(formData);
            
            if (responseUrl.StartsWith("Bad API request"))
            {
                _logger.LogError($"Pastebin API error: {responseUrl}");
                throw new Core.ApiException($"Pastebin API error: {responseUrl}");
            }
            
            _logger.LogInformation($"Paste created successfully: {responseUrl}");
            return PasteResponse.FromUrl(responseUrl);
        }
        catch (Core.ApiException ex)
        {
            _logger.LogError(ex, $"Failed to create paste: {request.Title}");
            throw;
        }
    }
    
    public async Task<PasteResponse> ShareCurrentCodeAsync(PastePrivacy privacy = PastePrivacy.Unlisted,
        string expiration = "1H",
        [CallerFilePath] string filePath = "")
    {
        _logger.LogDebug($"ShareCurrentCodeAsync called from: {filePath}");
        
        if (!File.Exists(filePath)) {
            _logger.LogError($"Source file not found: {filePath}");
            throw new FileNotFoundException("The source code file could not be found.", filePath);
        }
        
        var codeContent = await File.ReadAllTextAsync(filePath);
        var fileName = Path.GetFileName(filePath);
        var fileExtension = Path.GetExtension(filePath).TrimStart('.').ToLower();

        var pasteRequest = new PasteRequest
        {
            Content = codeContent,
            Privacy = privacy,
            Expiration = expiration,
            Title = fileName,
            Format = GetPastebinFormat(fileExtension)
        };

        return await CreatePasteAsync(pasteRequest);
    }
    
    public async Task<List<PasteResponse>> ShareProjectAsync(string projectPath,
        string[]? extensionsToInclude = null,
        PastePrivacy privacy = PastePrivacy.Unlisted,
        string expiration = "1H")
    {
        _logger.LogInformation($"Sharing project from: {projectPath}");
        extensionsToInclude ??= [".cs", ".csproj", ".json"];
        
        var files = Directory.GetFiles(projectPath, "*.*", SearchOption.AllDirectories)
            .Where(f => extensionsToInclude.Contains(Path.GetExtension(f)))
            .ToList();
    
        _logger.LogInformation($"Found {files.Count} files to share");
        var responses = new List<PasteResponse>();
    
        foreach (var file in files)
        {
            using (_logger.BeginScope("Sharing file {FileName}", file))
            {
                try
                {
                    var response = await ShareCurrentCodeAsync(privacy, expiration, file);
                    responses.Add(response);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to share file");
                }
            }
        }
    
        _logger.LogInformation($"Successfully shared {responses.Count} out of {files.Count} files");
        return responses;
    }

    private static string GetPastebinFormat(string extension)
    {
        return extension switch
        {
            "cs" => "csharp",
            "js" => "javascript",
            "py" => "python",
            "json" => "json",
            "xml" => "xml",
            "html" => "html",
            _ => "text"
        };
    }
}
