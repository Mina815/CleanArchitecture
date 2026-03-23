using CleanArchitecture.Application.Bookings.Commands.CancelBooking;
using CleanArchitecture.Application.Bookings.Commands.CompleteBooking;
using CleanArchitecture.Application.Bookings.Commands.ConfirmBooking;
using CleanArchitecture.Application.Bookings.Commands.CreateBooking;
using CleanArchitecture.Application.Bookings.Queries.GetAvailableSlots;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CleanArchitecture.Web.Endpoints;

public class Bookings : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetAvailableSlots, "available-slots");

        var auth = groupBuilder.MapGroup("").RequireAuthorization();
        auth.MapPost(CreateBooking, "");
        auth.MapPost(ConfirmBooking, "{bookingId:int}/confirm");
        auth.MapPost(CompleteBooking, "{bookingId:int}/complete");
        auth.MapPost(CancelBooking, "{bookingId:int}/cancel");
    }

    [EndpointSummary("Available time slots")]
    public static async Task<Ok<IReadOnlyList<AvailableSlotDto>>> GetAvailableSlots(
        ISender sender,
        int branchId,
        int serviceId,
        DateOnly date,
        int? staffId)
    {
        var slots = await sender.Send(new GetAvailableSlotsQuery(branchId, serviceId, date, staffId));
        return TypedResults.Ok(slots);
    }

    [EndpointSummary("Create booking (customer)")]
    public static async Task<Created<int>> CreateBooking(ISender sender, CreateBookingCommand command)
    {
        var id = await sender.Send(command);
        return TypedResults.Created($"/api/{nameof(Bookings)}/{id}", id);
    }

    [EndpointSummary("Confirm booking (provider)")]
    public static async Task<NoContent> ConfirmBooking(ISender sender, int bookingId)
    {
        await sender.Send(new ConfirmBookingCommand(bookingId));
        return TypedResults.NoContent();
    }

    [EndpointSummary("Complete booking (provider)")]
    public static async Task<NoContent> CompleteBooking(ISender sender, int bookingId)
    {
        await sender.Send(new CompleteBookingCommand(bookingId));
        return TypedResults.NoContent();
    }

    [EndpointSummary("Cancel booking")]
    public static async Task<NoContent> CancelBooking(ISender sender, int bookingId, CancelBookingRequest? body)
    {
        await sender.Send(new CancelBookingCommand(bookingId, body?.Reason));
        return TypedResults.NoContent();
    }
}

public record CancelBookingRequest(string? Reason);
