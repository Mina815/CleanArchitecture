using CleanArchitecture.Application.Centers;
using CleanArchitecture.Application.Centers.Commands.CreateCenter;
using CleanArchitecture.Application.Centers.Commands.UpdateCenter;
using CleanArchitecture.Application.Centers.Queries.GetCenterBranches;
using CleanArchitecture.Application.Centers.Queries.GetCenterById;
using CleanArchitecture.Application.Centers.Queries.GetCenterServices;
using CleanArchitecture.Application.Centers.Queries.GetCenters;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CleanArchitecture.Web.Endpoints;

public class Centers : IEndpointGroup
{
    public static string? RoutePrefix => "/api/centers";

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetCenters);
        groupBuilder.MapGet(GetCenter, "{id}");
        groupBuilder.MapGet(GetCenterBranches, "{id}/branches");
        groupBuilder.MapGet(GetCenterServices, "{id}/services");
        groupBuilder.MapPost(CreateCenter).RequireAuthorization();
        groupBuilder.MapPut(UpdateCenter, "{id}").RequireAuthorization();
    }

    public static async Task<Ok<List<CenterDto>>> GetCenters(ISender sender, string? city, string? search)
        => TypedResults.Ok(await sender.Send(new GetCentersQuery(city, search)));

    public static async Task<Results<Ok<CenterDetailDto>, NotFound>> GetCenter(ISender sender, int id)
    {
        var center = await sender.Send(new GetCenterByIdQuery(id));
        return center is null ? TypedResults.NotFound() : TypedResults.Ok(center);
    }

    public static async Task<Ok<List<BranchDetailDto>>> GetCenterBranches(ISender sender, int id)
        => TypedResults.Ok(await sender.Send(new GetCenterBranchesQuery(id)));

    public static async Task<Ok<List<ServiceDetailDto>>> GetCenterServices(ISender sender, int id, int? categoryId)
        => TypedResults.Ok(await sender.Send(new GetCenterServicesQuery(id, categoryId)));

    public static async Task<Created<int>> CreateCenter(ISender sender, CreateCenterCommand command)
    {
        var id = await sender.Send(command);
        return TypedResults.Created($"/api/centers/{id}", id);
    }

    public static async Task<Results<NoContent, BadRequest>> UpdateCenter(ISender sender, int id, UpdateCenterCommand command)
    {
        if (id != command.Id) return TypedResults.BadRequest();
        await sender.Send(command);
        return TypedResults.NoContent();
    }
}
