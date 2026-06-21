using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.Web.Endpoints;

public class Uploads : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(UploadFile, "file").DisableAntiforgery();
    }

    public static async Task<Results<Ok<string>, BadRequest<string>>> UploadFile(
        IFormFile file, IFileStorageService fileStorage, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            return TypedResults.BadRequest("No file provided.");

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg" };
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(ext))
            return TypedResults.BadRequest("File type not allowed. Allowed: jpg, jpeg, png, gif, webp, svg.");

        await using var stream = file.OpenReadStream();
        var url = await fileStorage.SaveFileAsync(stream, file.FileName, file.ContentType, cancellationToken);
        return TypedResults.Ok(url);
    }
}
