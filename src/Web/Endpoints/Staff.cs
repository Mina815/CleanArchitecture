using CleanArchitecture.Application.Staff;
using CleanArchitecture.Application.Staff.Commands.CreateStaff;
using CleanArchitecture.Application.Staff.Commands.UpdateStaff;
using CleanArchitecture.Application.Staff.Queries.GetBranchStaff;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CleanArchitecture.Web.Endpoints;

public class Staff : IEndpointGroup
{
    public static string? RoutePrefix => "/api/staff";

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetBranchStaff, "branch/{branchId}");
        groupBuilder.MapPost(CreateStaff).RequireAuthorization();
        groupBuilder.MapPut(UpdateStaff, "{id}").RequireAuthorization();
    }

    public static async Task<Ok<List<StaffDto>>> GetBranchStaff(ISender sender, int branchId)
        => TypedResults.Ok(await sender.Send(new GetBranchStaffQuery(branchId)));

    public static async Task<Created<int>> CreateStaff(ISender sender, CreateStaffCommand command)
    {
        var id = await sender.Send(command);
        return TypedResults.Created($"/api/staff/{id}", id);
    }

    public static async Task<Results<NoContent, BadRequest>> UpdateStaff(ISender sender, int id, UpdateStaffCommand command)
    {
        if (id != command.Id) return TypedResults.BadRequest();
        await sender.Send(command);
        return TypedResults.NoContent();
    }
}
