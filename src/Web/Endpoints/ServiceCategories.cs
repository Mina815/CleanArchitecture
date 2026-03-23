using CleanArchitecture.Application.ServiceCategories.Queries.GetServiceCategories;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CleanArchitecture.Web.Endpoints;

public class ServiceCategories : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetServiceCategories);
    }

    [EndpointSummary("List service categories")]
    public static async Task<Ok<IReadOnlyList<ServiceCategoryDto>>> GetServiceCategories(ISender sender)
    {
        var list = await sender.Send(new GetServiceCategoriesQuery());
        return TypedResults.Ok(list);
    }
}
