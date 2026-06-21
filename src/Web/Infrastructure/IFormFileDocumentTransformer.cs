using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace CleanArchitecture.Web.Infrastructure;

internal sealed class IFormFileDocumentTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.Schemas ??= new Dictionary<string, IOpenApiSchema>();

        if (!document.Components.Schemas.ContainsKey("IFormFile"))
        {
            document.Components.Schemas["IFormFile"] = new OpenApiSchema
            {
                Type = JsonSchemaType.String,
                Format = "binary"
            };
        }

        return Task.CompletedTask;
    }
}
