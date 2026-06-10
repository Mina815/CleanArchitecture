using CleanArchitecture.Application.Services.Commands.CreateService;
using CleanArchitecture.Application.Services.Commands.DeleteService;
using CleanArchitecture.Application.Services.Commands.UpdateService;
using CleanArchitecture.Application.Services.Queries.GetCategories;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CleanArchitecture.Web.Endpoints;

public class Services : IEndpointGroup
{
    public static string? RoutePrefix => "/api/services";

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetCategories, "categories");
        groupBuilder.MapPost(CreateService).RequireAuthorization();
        groupBuilder.MapPut(UpdateService, "{id}").RequireAuthorization();
        groupBuilder.MapDelete(DeleteService, "{id}").RequireAuthorization();
    }

    public static async Task<Ok<List<CategoryDto>>> GetCategories(ISender sender)
        => TypedResults.Ok(await sender.Send(new GetCategoriesQuery()));

    public static async Task<Created<int>> CreateService(ISender sender, CreateServiceCommand command)
    {
        var id = await sender.Send(command);
        return TypedResults.Created($"/api/services/{id}", id);
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
