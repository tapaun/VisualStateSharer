# VisualStateSharer

A modern .NET library for sharing code snippets and images to Pastebin and Gyazo using Refit and Dependency Injection.

## Features

- 📝 **PastebinApi** - Share code/text with syntax highlighting
- 🖼️ **GyazoApi** - Upload and share images
- 🔒 **Privacy controls** - Public, unlisted, or private pastes
- ⏱️ **Expiration settings** - Auto-delete after specified time
- 🔧 **Refit-powered** - Clean, declarative HTTP API definitions
- 💉 **Dependency Injection** - Proper DI setup with HttpClientFactory
- 📊 **Built-in logging** - Track uploads and errors via ILogger
- 🎯 **Auto-detection** - Automatically captures calling file with `[CallerFilePath]`

## Installation

### Via NuGet (when published)
```bash
dotnet add package VisualStateSharer
```

### Via Project Reference
```bash
dotnet add reference path/to/VisualStateSharer/VisualStateSharer.csproj
```

## Setup

### 1. Configure Environment Variables

Create a `.env` file or set system environment variables:

```env
PASTEBIN_API_KEY=your_pastebin_api_key
GYAZO_ACCESS_TOKEN=your_gyazo_access_token
```

**Get your API keys:**
- **Pastebin**: https://pastebin.com/doc_api#1
- **Gyazo**: https://gyazo.com/oauth/applications (Create application → Get access token)

### 2. Register Services (ASP.NET Core / Generic Host)

```csharp
using VisualStateSharer.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Register VisualStateSharer services
builder.Services.AddVisualStateSharer(builder.Configuration);

var app = builder.Build();
```

### 3. Register Services (Console App)

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VisualStateSharer.Extensions;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddVisualStateSharer(context.Configuration);
    })
    .Build();

// Get services from DI container
var pastebinApi = host.Services.GetRequiredService<PastebinApi>();
var gyazoApi = host.Services.GetRequiredService<GyazoApi>();
```

## Usage

### PastebinApi

**Share current file automatically:**
```csharp
public class MyService
{
    private readonly PastebinApi _pastebinApi;
    
    public MyService(PastebinApi pastebinApi)
    {
        _pastebinApi = pastebinApi;
    }
    
    public async Task ShareCode()
    {
        // CallerFilePath automatically captures this file
        var response = await _pastebinApi.ShareCurrentCodeAsync(
            privacy: PastePrivacy.Unlisted,
            expiration: "1H"  // Options: N, 10M, 1H, 1D, 1W, 2W, 1M, 6M, 1Y
        );
        
        Console.WriteLine($"Code shared: {response.Url}");
    }
}
```

**Share specific file:**
```csharp
var response = await pastebinApi.ShareCurrentCodeAsync(
    privacy: PastePrivacy.Private,
    expiration: "1D",
    filePath: "/path/to/MyClass.cs"
);
```

**Share entire project:**
```csharp
var responses = await pastebinApi.ShareProjectAsync(
    projectPath: "./MyProject",
    extensionsToInclude: new[] { ".cs", ".json", ".csproj" },
    privacy: PastePrivacy.Unlisted,
    expiration: "1W"
);

Console.WriteLine($"Shared {responses.Count} files");
```

**Create custom paste:**
```csharp
var request = new PasteRequest
{
    Content = "Console.WriteLine(\"Hello World\");",
    Title = "Hello World Example",
    Privacy = PastePrivacy.Public,
    Expiration = "1M",
    Format = "csharp"
};

var response = await pastebinApi.CreatePasteAsync(request);
```

### GyazoApi

**Upload image:**
```csharp
public class ImageService
{
    private readonly GyazoApi _gyazoApi;
    
    public ImageService(GyazoApi gyazoApi)
    {
        _gyazoApi = gyazoApi;
    }
    
    public async Task UploadImage()
    {
        var request = new UploadRequest
        {
            ImagePath = "/path/to/screenshot.png",
            Title = "My Screenshot",
            Description = "Optional description",
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };
        
        var response = await _gyazoApi.UploadAsync(request);
        Console.WriteLine($"Image uploaded: {response.PermalinkUrl}");
    }
}
```

**Share screenshot:**
```csharp
var response = await gyazoApi.ShareScreenshotAsync("/path/to/screenshot.png");
Console.WriteLine($"Screenshot shared: {response.PermalinkUrl}");
```

## Configuration

### appsettings.json (Optional)

```json
{
  "Pastebin": {
    "BaseUrl": "https://pastebin.com/api",
    "ApiKey": "your-key-here",
    "Timeout": 30
  },
  "Gyazo": {
    "BaseUrl": "https://upload.gyazo.com/api",
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret",
    "Timeout": 30
  }
}
```

*Note: Currently, API keys are read from environment variables. Configuration binding is set up for future extensibility.*

## Models

### PastePrivacy
- `Public` (0) - Visible to everyone
- `Unlisted` (1) - Only accessible via URL
- `Private` (2) - Only visible to you

### Expiration Options
- `N` - Never
- `10M` - 10 Minutes
- `1H` - 1 Hour
- `1D` - 1 Day
- `1W` - 1 Week
- `2W` - 2 Weeks
- `1M` - 1 Month
- `6M` - 6 Months
- `1Y` - 1 Year

## Architecture

This library uses:
- **Refit** - Type-safe HTTP client library
- **HttpClientFactory** - Proper HttpClient management and pooling
- **Microsoft.Extensions.DependencyInjection** - Standard .NET DI
- **Microsoft.Extensions.Logging** - Integrated logging support

## Error Handling

All methods throw `ApiException` on failure:

```csharp
try
{
    var response = await pastebinApi.ShareCurrentCodeAsync();
}
catch (ApiException ex)
{
    Console.WriteLine($"API Error: {ex.Message}");
}
catch (FileNotFoundException ex)
{
    Console.WriteLine($"File not found: {ex.Message}");
}
```

## Requirements

- .NET 8.0 or .NET 9.0
- Valid Pastebin API key
- Valid Gyazo access token

## License

This project is licensed under the MIT License.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.
