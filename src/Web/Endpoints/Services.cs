using CleanArchitecture.Application.Services.Commands.CreateService;
using CleanArchitecture.Application.Services.Commands.DeleteService;
using CleanArchitecture.Application.Services.Commands.UpdateService;
using CleanArchitecture.Application.Services.Queries.GetCenterServices;

namespace CleanArchitecture.Web.Endpoints;

public class Services : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetServices, "center/{centerId}");
        groupBuilder.MapPost(CreateService);
        groupBuilder.MapPut(UpdateService, "{id}");
        groupBuilder.MapDelete(DeleteService, "{id}");
    }

    public static async Task<Ok<List<ServiceDto>>> GetServices(ISender sender, int centerId)
    {
        var result = await sender.Send(new GetCenterServicesQuery(centerId));
        return TypedResults.Ok(result);
    }

    public static async Task<Created<int>> CreateService(ISender sender, CreateServiceCommand command)
    {
        var id = await sender.Send(command);
        return TypedResults.Created($"/api/Services/{id}", id);
    }

    public static async Task<Results<NoContent, BadRequest>> UpdateService(ISender sender, int id, UpdateServiceCommand command)
    {
        if (id != command.Id) return TypedResults.BadRequest();
        await sender.Send(command);
        return TypedResults.NoContent();
    }

    public static async Task<NoContent> DeleteService(ISender sender, int id)
    {
        await sender.Send(new DeleteServiceCommand(id));
        return TypedResults.NoContent();
    }
}
