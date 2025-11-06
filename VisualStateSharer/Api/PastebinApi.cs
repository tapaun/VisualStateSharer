using System.Reflection;
using System.Runtime.CompilerServices;
using VisualStateSharer.Core;
using VisualStateSharer.Models.Pastebin;
using VisualStateSharer.Utils;

namespace VisualStateSharer.Api;

public class PastebinApi(string baseUrl, string apiKey) : ApiClient(baseUrl, apiKey) {
    private static readonly string CreatePasteEndpoint = ApiConfig.PastebinPostEndpoint;

    private async Task<PasteResponse> CreatePasteAsync(PasteRequest request)
    {
        Logger.Info($"Creating paste: {request.Title}");
        
        var formData = new Dictionary<string, string?>
        {
            { "api_dev_key", ApiKey },
            { "api_option", "paste" },
            { "api_paste_code", request.Content },
            { "api_paste_private", ((int)request.Privacy).ToString() },
            { "api_paste_expire_date", request.Expiration },
            { "api_paste_name", request.Title },
            { "api_paste_format", request.Format }
        };

        try
        {
            var content = new FormUrlEncodedContent(formData);
            var fullUrl = GetFullUrl(CreatePasteEndpoint);
            Logger.Debug($"Posting to: {fullUrl}");
            
            using var client = new HttpClient();
            var response = await client.PostAsync(fullUrl, content);
            var responseBody = await response.Content.ReadAsStringAsync();
            
            Logger.Debug($"Response status: {response.StatusCode}");
            Logger.Debug($"Response body: {responseBody}");
            
            if (!response.IsSuccessStatusCode)
            {
                Logger.Error($"Pastebin API error ({response.StatusCode}): {responseBody}");
                throw new ApiException($"Pastebin API error ({response.StatusCode}): {responseBody}");
            }
            
            if (responseBody.StartsWith("Bad API request"))
            {
                Logger.Error($"Pastebin API error: {responseBody}");
                throw new ApiException($"Pastebin API error: {responseBody}");
            }
            
            Logger.Info($"Paste created successfully: {responseBody}");
            return PasteResponse.FromUrl(responseBody);
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to create paste: {request.Title}", ex);
            throw;
        }
    }
    
    public async Task<PasteResponse> ShareCurrentCodeAsync(PastePrivacy privacy = PastePrivacy.Unlisted,
        string expiration = "1H",
        [CallerFilePath] string filePath = "")
    {
        Logger.Debug($"ShareCurrentCodeAsync called from: {filePath}");
        
        if (!File.Exists(filePath)) {
            Logger.Error($"Source file not found: {filePath}");
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
        string[] extensionsToInclude = null,
        PastePrivacy privacy = PastePrivacy.Unlisted,
        string expiration = "1H")
    {
        Logger.Info($"Sharing project from: {projectPath}");
        extensionsToInclude ??= [".cs", ".csproj", ".json"];
        
        var files = Directory.GetFiles(projectPath, "*.*", SearchOption.AllDirectories)
            .Where(f => extensionsToInclude.Contains(Path.GetExtension(f)))
            .ToList();
    
        Logger.Info($"Found {files.Count} files to share");
        var responses = new List<PasteResponse>();
    
        foreach (var file in files)
        {
            try
            {
                // ReSharper disable once ExplicitCallerInfoArgument
                var response = await ShareCurrentCodeAsync(privacy, expiration, file);
                responses.Add(response);
            }
            catch (Exception ex)
            {
                Logger.Warning($"Failed to share file {file}: {ex.Message}");
            }
        }
    
        Logger.Info($"Successfully shared {responses.Count} out of {files.Count} files");
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
