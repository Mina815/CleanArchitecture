using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.Shops.Commands.CreateShop;
//using CleanArchitecture.Application.Shops.Commands.DeleteShop;
//using CleanArchitecture.Application.Shops.Commands.UpdateShop;
//using CleanArchitecture.Application.Shops.Commands.UpdateShopDetail;
using CleanArchitecture.Application.Shops.Queries.GetShopsWithPagination;
using CleanArchitecture.Application.TodoItems.Queries.GetTodoItemsWithPagination;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CleanArchitecture.Web.Endpoints;

public class Shops : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetShopsWithPagination).RequireAuthorization();
        groupBuilder.MapPost(CreateShop).RequireAuthorization();
        //groupBuilder.MapPut(UpdateShop, "{id}").RequireAuthorization();
        //groupBuilder.MapPatch(UpdateShopDetail, "UpdateDetail/{id}").RequireAuthorization();
        //groupBuilder.MapDelete(DeleteShop, "{id}").RequireAuthorization();
    }

    public async Task<Ok<PaginatedList<ShopBriefDto>>> GetShopsWithPagination(ISender sender, [AsParameters] GetShopsWithPaginationQuery query)
    {
        var result = await sender.Send(query);

        return TypedResults.Ok(result);
    }

    public async Task<Created<int>> CreateShop(ISender sender, CreateShopCommand command)
    {
        var id = await sender.Send(command);

        return TypedResults.Created($"/{nameof(Shops)}/{id}", id);
    }

    //public async Task<Results<NoContent, BadRequest>> UpdateShop(ISender sender, int id, UpdateShopCommand command)
    //{
    //    if (id != command.Id) return TypedResults.BadRequest();

    //    await sender.Send(command);

    //    return TypedResults.NoContent();
    //}

    //public async Task<Results<NoContent, BadRequest>> UpdateShopDetail(ISender sender, int id, UpdateShopDetailCommand command)
    //{
    //    if (id != command.Id) return TypedResults.BadRequest();

    //    await sender.Send(command);

    //    return TypedResults.NoContent();
    //}

    //public async Task<NoContent> DeleteShop(ISender sender, int id)
    //{
    //    await sender.Send(new DeleteShopCommand(id));

    //    return TypedResults.NoContent();
    //}
}
