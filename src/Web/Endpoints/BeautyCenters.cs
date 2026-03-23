using CleanArchitecture.Application.BeautyCenters.Commands.CreateBeautyCenter;
using CleanArchitecture.Application.BeautyCenters.Queries.GetCenters;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CleanArchitecture.Web.Endpoints;

public class BeautyCenters : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetCenters);

        var auth = groupBuilder.MapGroup("").RequireAuthorization();
        auth.MapPost(CreateCenter, "");
    }

    [EndpointSummary("List beauty centers")]
    public static async Task<Ok<IReadOnlyList<CenterBriefDto>>> GetCenters(ISender sender, string? city)
    {
        var list = await sender.Send(new GetCentersQuery(city));
        return TypedResults.Ok(list);
    }

    [EndpointSummary("Create beauty center (provider)")]
    public static async Task<Created<int>> CreateCenter(ISender sender, CreateBeautyCenterCommand command)
    {
        var id = await sender.Send(command);
        return TypedResults.Created($"/api/{nameof(BeautyCenters)}/{id}", id);
    }
}
