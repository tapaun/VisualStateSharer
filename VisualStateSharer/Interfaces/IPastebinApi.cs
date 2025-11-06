using Refit;

namespace VisualStateSharer.Interfaces;

public interface IPastebinApi
{
    [Post("/api_post.php")]
    Task<string> RefitCreatePasteAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, string> formData);
}
