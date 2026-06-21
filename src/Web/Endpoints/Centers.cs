using CleanArchitecture.Application.Centers.Commands.CreateCenter;
using CleanArchitecture.Application.Centers.Commands.UpdateCenter;
using CleanArchitecture.Application.Centers.Queries.GetCenterById;
using CleanArchitecture.Application.Centers.Queries.GetCenters;
using CleanArchitecture.Application.Centers.Queries.GetMyCenter;

namespace CleanArchitecture.Web.Endpoints;

public class Centers : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetCenters);
        groupBuilder.MapGet(GetMyCenterEndpoint, "mine");
        groupBuilder.MapGet(GetCenterById, "{id}");
        groupBuilder.MapPost(CreateCenter);
        groupBuilder.MapPut(UpdateCenter, "{id}");
    }

    public static async Task<Ok<PaginatedList<CenterDto>>> GetCenters(ISender sender, [AsParameters] GetCentersQuery query)
    {
        var result = await sender.Send(query);
        return TypedResults.Ok(result);
    }

    public static async Task<Ok<CenterDetailDto>> GetMyCenterEndpoint(ISender sender)
    {
        var result = await sender.Send(new GetMyCenterQuery());
        return TypedResults.Ok(result);
    }

    public static async Task<Results<Ok<CenterDetailDto>, NotFound>> GetCenterById(ISender sender, int id)
    {
        var result = await sender.Send(new GetCenterByIdQuery(id));
        return TypedResults.Ok(result);
    }

    public static async Task<Created<int>> CreateCenter(ISender sender, CreateCenterCommand command)
    {
        var id = await sender.Send(command);
        return TypedResults.Created($"/api/Centers/{id}", id);
    }

    public static async Task<Results<NoContent, BadRequest>> UpdateCenter(ISender sender, int id, UpdateCenterCommand command)
    {
        if (id != command.Id) return TypedResults.BadRequest();
        await sender.Send(command);
        return TypedResults.NoContent();
    }
}
