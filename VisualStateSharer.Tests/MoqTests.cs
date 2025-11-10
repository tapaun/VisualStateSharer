using Microsoft.Extensions.Logging;
using Moq;
using Refit;
using VisualStateSharer.Api;
using VisualStateSharer.Interfaces;
using VisualStateSharer.Models.Gyazo;
using VisualStateSharer.Models.Pastebin;

namespace VisualStateSharer.Tests;

public class MoqTests {
    [Fact]
    public async Task PastebinApi_CreatePaste_ShouldReturnPasteResponse()
    {
        // Arrange
        var mockApi = new Mock<IPastebinApi>();
        var mockLogger = new Mock<ILogger<PastebinApi>>();
        var testApiKey = "test-api-key";
        
        mockApi.Setup(x => 
                x.RefitCreatePasteAsync(It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync("https://pastebin.com/test123");
        
        var pastebinApi = new PastebinApi(mockApi.Object, testApiKey, mockLogger.Object);
        
        var request = new PasteRequest
        {
            Content = "Console.WriteLine(\"Hello World\");",
            Title = "Test.cs",
            Privacy = PastePrivacy.Unlisted,
            Expiration = "1H",
            Format = "csharp"
        };
        
        // Act
        var result = await pastebinApi.CreatePasteAsync(request);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal("https://pastebin.com/test123", result.Url);
    }
    
    [Fact]
    public async Task GyazoApi_UploadImage_ShouldReturnImageInfo()
    {
        // Arrange
        var mockApi = new Mock<IGyazoApi>();
        var mockLogger = new Mock<ILogger<GyazoApi>>();
        var testAccessToken = "test-access-token";
        
        var expectedImageInfo = new ImageInfo
        {
            ImageId = "test-image-id",
            PermalinkUrl = "https://gyazo.com/test-image-id",
            ThumbUrl = "https://thumb.gyazo.com/test-image-id",
            Url = "https://i.gyazo.com/test-image-id.png",
            Type = "png",
            CreatedAt = DateTime.UtcNow.ToString("o")
        };
        
        mockApi.Setup(x => x.RefitUploadImageAsync(
            It.IsAny<string>(),
            It.IsAny<StreamPart>(),
            It.IsAny<string>(),
            It.IsAny<string>()))
            .ReturnsAsync(expectedImageInfo);
        
        var gyazoApi = new GyazoApi(mockApi.Object, testAccessToken, mockLogger.Object);
        
        // Create a temporary test image file
        var tempImagePath = Path.Combine(Path.GetTempPath(), "test-image.png");
        await File.WriteAllBytesAsync(tempImagePath, new byte[] { 0x89, 0x50, 0x4E, 0x47 }); // PNG header
        
        var request = new UploadRequest
        {
            ImagePath = tempImagePath,
            Title = "Test Image",
            Description = "Test Description"
        };
        
        // Act
        var result = await gyazoApi.UploadImageAsync(request);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal("https://gyazo.com/test-image-id", result.PermalinkUrl);
        Assert.Equal("test-image-id", result.ImageId);
        
        // Cleanup
        if (File.Exists(tempImagePath))
            File.Delete(tempImagePath);
    }
}