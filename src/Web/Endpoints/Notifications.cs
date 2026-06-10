using CleanArchitecture.Application.Notifications.Commands.MarkAllNotificationsRead;
using CleanArchitecture.Application.Notifications.Commands.MarkNotificationRead;
using CleanArchitecture.Application.Notifications.Queries.GetNotifications;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CleanArchitecture.Web.Endpoints;

public class Notifications : IEndpointGroup
{
    public static string? RoutePrefix => "/api/notifications";

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.RequireAuthorization();
        groupBuilder.MapGet(GetNotifications);
        groupBuilder.MapPut(MarkRead, "{id}/mark-read");
        groupBuilder.MapPut(MarkAllRead, "mark-all-read");
    }

    public static async Task<Ok<List<NotificationDto>>> GetNotifications(ISender sender, bool unreadOnly = false)
        => TypedResults.Ok(await sender.Send(new GetNotificationsQuery(unreadOnly)));

    public static async Task<NoContent> MarkRead(ISender sender, int id)
    {
        await sender.Send(new MarkNotificationReadCommand(id));
        return TypedResults.NoContent();
    }

    public static async Task<NoContent> MarkAllRead(ISender sender)
    {
        await sender.Send(new MarkAllNotificationsReadCommand());
        return TypedResults.NoContent();
    }
}
