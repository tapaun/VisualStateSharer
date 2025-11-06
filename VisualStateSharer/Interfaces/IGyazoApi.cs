using System.Net;
using Refit;
using VisualStateSharer.Models.Gyazo;

namespace VisualStateSharer.Interfaces;

public interface IGyazoApi
{
    [Multipart]
    [Post("/upload")]
    Task<ImageInfo> RefitUploadImageAsync([AliasAs("access_token")] string accessToken,
        [AliasAs("imagedata")] StreamPart imageData,
        [AliasAs("title")] string? title = null,
        [AliasAs("desc")] string? description = null);
}
