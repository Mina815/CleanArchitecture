using CleanArchitecture.Application.Branches.Commands.CreateBranch;
using CleanArchitecture.Application.Branches.Commands.UpdateBranch;
using CleanArchitecture.Application.Branches.Queries.GetCenterBranches;

namespace CleanArchitecture.Web.Endpoints;

public class Branches : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetBranches, "center/{centerId}");
        groupBuilder.MapPost(CreateBranch);
        groupBuilder.MapPut(UpdateBranch, "{id}");
    }

    public static async Task<Ok<List<BranchDto>>> GetBranches(ISender sender, int centerId)
    {
        var result = await sender.Send(new GetCenterBranchesQuery(centerId));
        return TypedResults.Ok(result);
    }

    public static async Task<Created<int>> CreateBranch(ISender sender, CreateBranchCommand command)
    {
        var id = await sender.Send(command);
        return TypedResults.Created($"/api/Branches/{id}", id);
    }

    public static async Task<Results<NoContent, BadRequest>> UpdateBranch(ISender sender, int id, UpdateBranchCommand command)
    {
        if (id != command.Id) return TypedResults.BadRequest();
        await sender.Send(command);
        return TypedResults.NoContent();
    }
}
