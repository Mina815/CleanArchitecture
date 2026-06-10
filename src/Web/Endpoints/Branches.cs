using CleanArchitecture.Application.Branches.Commands.CreateBranch;
using CleanArchitecture.Application.Branches.Commands.CreateTimeOff;
using CleanArchitecture.Application.Branches.Commands.SetWorkingHours;
using CleanArchitecture.Application.Branches.Commands.UpdateBranch;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CleanArchitecture.Web.Endpoints;

public class Branches : IEndpointGroup
{
    public static string? RoutePrefix => "/api/branches";

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(CreateBranch).RequireAuthorization();
        groupBuilder.MapPut(UpdateBranch, "{id}").RequireAuthorization();
        groupBuilder.MapPost(SetWorkingHours, "{id}/working-hours").RequireAuthorization();
        groupBuilder.MapPost(CreateTimeOff, "{id}/time-off").RequireAuthorization();
    }

    public static async Task<Created<int>> CreateBranch(ISender sender, CreateBranchCommand command)
    {
        var id = await sender.Send(command);
        return TypedResults.Created($"/api/branches/{id}", id);
    }

    public static async Task<Results<NoContent, BadRequest>> UpdateBranch(ISender sender, int id, UpdateBranchCommand command)
    {
        if (id != command.Id) return TypedResults.BadRequest();
        await sender.Send(command);
        return TypedResults.NoContent();
    }

    public static async Task<NoContent> SetWorkingHours(ISender sender, int id, SetWorkingHoursRequest request)
    {
        await sender.Send(new SetWorkingHoursCommand { BranchId = id, Hours = request.Hours });
        return TypedResults.NoContent();
    }

    public static async Task<Created<int>> CreateTimeOff(ISender sender, int id, CreateTimeOffCommand command)
    {
        var cmd = command with { BranchId = id };
        var timeOffId = await sender.Send(cmd);
        return TypedResults.Created($"/api/branches/{id}/time-off/{timeOffId}", timeOffId);
    }

    public record SetWorkingHoursRequest(List<Application.Branches.Commands.SetWorkingHours.WorkingHourDto> Hours);
}
