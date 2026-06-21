using CleanArchitecture.Application.Branches.Commands.CreateBranch;
using CleanArchitecture.Application.Branches.Commands.CreateTimeOff;
using CleanArchitecture.Application.Branches.Commands.SetWorkingHours;
using CleanArchitecture.Application.Branches.Commands.UpdateBranch;
using CleanArchitecture.Application.Branches.Queries.GetBranchTimeOffs;
using CleanArchitecture.Application.Branches.Queries.GetBranchWorkingHours;
using CleanArchitecture.Application.Branches.Queries.GetCenterBranches;

namespace CleanArchitecture.Web.Endpoints;

public class Branches : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetBranches, "center/{centerId}");
        groupBuilder.MapPost(CreateBranch);
        groupBuilder.MapPut(UpdateBranch, "{id}");
        groupBuilder.MapPost(SetBranchWorkingHours, "{branchId}/hours");
        groupBuilder.MapGet(GetBranchWorkingHoursEndpoint, "{branchId}/hours");
        groupBuilder.MapPost(CreateBranchTimeOff, "{branchId}/timeoff");
        groupBuilder.MapGet(GetBranchTimeOffsEndpoint, "{branchId}/timeoffs");
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

    public static async Task<Results<NoContent, BadRequest>> SetBranchWorkingHours(ISender sender, int branchId, SetWorkingHoursCommand command)
    {
        if (branchId != command.BranchId) return TypedResults.BadRequest();
        await sender.Send(command);
        return TypedResults.NoContent();
    }

    public static async Task<Ok<List<WorkingHourDto>>> GetBranchWorkingHoursEndpoint(ISender sender, int branchId)
    {
        var result = await sender.Send(new GetBranchWorkingHoursQuery(branchId));
        return TypedResults.Ok(result);
    }

    public static async Task<Results<Created<int>, BadRequest>> CreateBranchTimeOff(ISender sender, int branchId, CreateTimeOffCommand command)
    {
        if (branchId != command.BranchId) return TypedResults.BadRequest();
        var id = await sender.Send(command);
        return TypedResults.Created($"/api/Branches/{branchId}/timeoff/{id}", id);
    }

    public static async Task<Ok<List<TimeOffDto>>> GetBranchTimeOffsEndpoint(ISender sender, int branchId)
    {
        var result = await sender.Send(new GetBranchTimeOffsQuery(branchId));
        return TypedResults.Ok(result);
    }
}
