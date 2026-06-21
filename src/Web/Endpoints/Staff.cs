using CleanArchitecture.Application.Staff.Commands.CreateStaff;
using CleanArchitecture.Application.Staff.Commands.DeleteStaff;
using CleanArchitecture.Application.Staff.Commands.SetStaffServices;
using CleanArchitecture.Application.Staff.Commands.UpdateStaff;
using CleanArchitecture.Application.Staff.Queries.GetBranchStaff;

namespace CleanArchitecture.Web.Endpoints;

public class Staff : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetBranchStaff, "branch/{branchId}");
        groupBuilder.MapPost(CreateStaff);
        groupBuilder.MapPut(UpdateStaff, "{id}");
        groupBuilder.MapDelete(DeleteStaff, "{id}");
        groupBuilder.MapPut(SetStaffServices, "{staffId}/services");
    }

    public static async Task<Ok<List<StaffDto>>> GetBranchStaff(ISender sender, int branchId)
    {
        var result = await sender.Send(new GetBranchStaffQuery(branchId));
        return TypedResults.Ok(result);
    }

    public static async Task<Created<int>> CreateStaff(ISender sender, CreateStaffCommand command)
    {
        var id = await sender.Send(command);
        return TypedResults.Created($"/api/Staff/{id}", id);
    }

    public static async Task<Results<NoContent, BadRequest>> UpdateStaff(ISender sender, int id, UpdateStaffCommand command)
    {
        if (id != command.Id) return TypedResults.BadRequest();
        await sender.Send(command);
        return TypedResults.NoContent();
    }

    public static async Task<NoContent> DeleteStaff(ISender sender, int id)
    {
        await sender.Send(new DeleteStaffCommand(id));
        return TypedResults.NoContent();
    }

    public static async Task<Results<NoContent, BadRequest>> SetStaffServices(ISender sender, int staffId, SetStaffServicesCommand command)
    {
        if (staffId != command.StaffId) return TypedResults.BadRequest();
        await sender.Send(command);
        return TypedResults.NoContent();
    }
}
