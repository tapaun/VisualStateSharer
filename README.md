# VisualStateSharer

A .NET 9.0 library for sharing code and images to Pastebin and Gyazo.

## Features

- 📝 **PastebinApi** - Share code/text with syntax highlighting
- 🖼️ **GyazoApi** - Upload and share images
- 🔒 **Privacy controls** - Public, unlisted, or private pastes
- ⏱️ **Expiration settings** - Auto-delete after specified time
- 📊 **Built-in logging** - Track uploads and errors
- 🎯 **Auto-detection** - Automatically captures calling file with `[CallerFilePath]`

## Installation

```bash
dotnet add package VisualStateSharer
```

## Quick Start

### Setup Environment Variables

Create a `.env` file:

```env
PASTEBIN_API_KEY=your_pastebin_api_key
GYAZO_CLIENT_ID=your_gyazo_client_id
GYAZO_CLIENT_SECRET=your_gyazo_client_secret
```

Get your API keys:
- **Pastebin**: https://pastebin.com/doc_api#1
- **Gyazo**: https://gyazo.com/oauth/applications

### PastebinApi Usage

```csharp
using VisualStateSharer.Api;
using VisualStateSharer.Models.Pastebin;

// Automatically share the current file
var api = new PastebinApi("https://pastebin.com/api/", apiKey);
var response = await api.ShareCurrentCodeAsync(
    privacy: PastePrivacy.Unlisted,
    expiration: "1H"  // N, 10M, 1H, 1D, 1W, 2W, 1M, 6M, 1Y
);

Console.WriteLine($"Shared: {response.Url}");
```

**Share specific file:**

```csharp
var response = await api.ShareCurrentCodeAsync(
    privacy: PastePrivacy.Private,
    expiration: "1D",
    filePath: "/path/to/file.cs"
);
```

**Share entire project:**

```csharp
var responses = await api.ShareProjectAsync(
    projectPath: "./MyProject",
    extensionsToInclude: new[] { ".cs", ".json", ".csproj" }
);
```

### GyazoApi Usage

```csharp
using VisualStateSharer.Api;
using VisualStateSharer.Models.Gyazo;

// Get access token
var auth = new GyazoAuth(clientId, clientSecret);
var accessToken = await auth.GetAccessTokenAsync();

// Upload image
var api = new GyazoApi("https://upload.gyazo.com/api/", accessToken);
var request = new UploadRequest
{
    ImagePath = "/path/to/image.png",
    Title = "My Screenshot",
    Description = "Optional description"
};

var response = await api.UploadImageAsync(request);
Console.WriteLine($"Uploaded: {response.PermalinkUrl}");
```

**Convenience method for screenshots:**

```csharp
var response = await api.ShareScreenshotAsync("/path/to/screenshot.png");
```

## Privacy Options

```csharp
PastePrivacy.Public    // Searchable and listed
PastePrivacy.Unlisted  // Accessible via URL only
PastePrivacy.Private   // Requires login to view
```

## Logging

Enable/disable logging:

```csharp
using VisualStateSharer.Utils;

Logger.Enable();   // On by default
Logger.Disable();  // Turn off logging
```

## Architecture

```
VisualStateSharer/
├── Api/              # API implementations
├── Core/             # Base classes and config
├── Models/           # Request/response models
├── Interfaces/       # Service contracts
└── Utils/            # Logger and JSON helpers
```

## License

MIT

## Contributing

Pull requests welcome! Please ensure your code follows the existing architecture.

---

Made with ❤️ for developers who want to share their visual state quickly.

