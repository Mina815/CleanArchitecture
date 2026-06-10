using CleanArchitecture.Application.Bookings.Commands.CancelBooking;
using CleanArchitecture.Application.Bookings.Commands.CompleteBooking;
using CleanArchitecture.Application.Bookings.Commands.ConfirmBooking;
using CleanArchitecture.Application.Bookings.Commands.CreateBooking;
using CleanArchitecture.Application.Bookings.Queries.GetAvailability;
using CleanArchitecture.Application.Bookings.Queries.GetBookingById;
using CleanArchitecture.Application.Bookings.Queries.GetBranchTodayBookings;
using CleanArchitecture.Application.Bookings.Queries.GetMyBookings;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CleanArchitecture.Web.Endpoints;

public class Bookings : IEndpointGroup
{
    public static string? RoutePrefix => "/api/bookings";

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(CreateBooking).RequireAuthorization();
        groupBuilder.MapGet(GetBooking, "{id}");
        groupBuilder.MapGet(GetMyBookings, "my-bookings").RequireAuthorization();
        groupBuilder.MapGet(GetBranchTodayBookings, "branch/{branchId}/today").RequireAuthorization();
        groupBuilder.MapGet(GetAvailability, "availability");
        groupBuilder.MapPut(ConfirmBooking, "{id}/confirm").RequireAuthorization();
        groupBuilder.MapPut(CancelBooking, "{id}/cancel").RequireAuthorization();
        groupBuilder.MapPut(CompleteBooking, "{id}/complete").RequireAuthorization();
    }

    public static async Task<Created<int>> CreateBooking(ISender sender, CreateBookingCommand command)
    {
        var id = await sender.Send(command);
        return TypedResults.Created($"/api/bookings/{id}", id);
    }

    public static async Task<Results<Ok<Application.Bookings.BookingDto>, NotFound>> GetBooking(ISender sender, int id)
    {
        var booking = await sender.Send(new GetBookingByIdQuery(id));
        return booking is null ? TypedResults.NotFound() : TypedResults.Ok(booking);
    }

    public static async Task<Ok<List<Application.Bookings.BookingDto>>> GetMyBookings(ISender sender)
        => TypedResults.Ok(await sender.Send(new GetMyBookingsQuery()));

    public static async Task<Ok<List<Application.Bookings.BookingDto>>> GetBranchTodayBookings(ISender sender, int branchId)
        => TypedResults.Ok(await sender.Send(new GetBranchTodayBookingsQuery(branchId)));

    public static async Task<Ok<List<Application.Common.Interfaces.TimeSlotDto>>> GetAvailability(
        ISender sender, int branchId, int serviceId, DateOnly date, int? staffId)
        => TypedResults.Ok(await sender.Send(new GetAvailabilityQuery(branchId, serviceId, date, staffId)));

    public static async Task<NoContent> ConfirmBooking(ISender sender, int id)
    {
        await sender.Send(new ConfirmBookingCommand(id));
        return TypedResults.NoContent();
    }

    public static async Task<NoContent> CancelBooking(ISender sender, int id, CancelBookingRequest? body)
    {
        await sender.Send(new CancelBookingCommand(id, body?.Reason));
        return TypedResults.NoContent();
    }

    public static async Task<NoContent> CompleteBooking(ISender sender, int id)
    {
        await sender.Send(new CompleteBookingCommand(id));
        return TypedResults.NoContent();
    }

    public record CancelBookingRequest(string? Reason);
}
