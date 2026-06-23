using CleanArchitecture.Application.Bookings.Commands.CancelBooking;
using CleanArchitecture.Application.Bookings.Commands.CompleteBooking;
using CleanArchitecture.Application.Bookings.Commands.ConfirmBooking;
using CleanArchitecture.Application.Bookings.Commands.CreateBooking;
using CleanArchitecture.Application.Bookings.Queries.GetAvailableSlots;
using CleanArchitecture.Application.Bookings.Queries.GetBookingById;
using CleanArchitecture.Application.Bookings.Queries.GetBranchBookings;
using CleanArchitecture.Application.Bookings.Queries.GetMyBookings;

namespace CleanArchitecture.Web.Endpoints;

public class Bookings : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetMyBookings);
        groupBuilder.MapGet(GetBookingById, "{id}");
        groupBuilder.MapGet(GetBranchBookings, "branch/{branchId}/bookings");
        groupBuilder.MapPost(CreateBooking);
        groupBuilder.MapGet(GetAvailableSlots, "slots");
        groupBuilder.MapPost(ConfirmBooking, "{id}/confirm");
        groupBuilder.MapPost(CompleteBooking, "{id}/complete");
        groupBuilder.MapPost(CancelBooking, "{id}/cancel");
    }

    public static async Task<Ok<List<BookingDto>>> GetMyBookings(ISender sender, bool? upcoming)
    {
        var result = await sender.Send(new GetMyBookingsQuery { Upcoming = upcoming });
        return TypedResults.Ok(result);
    }

    public static async Task<Ok<BookingDetailDto>> GetBookingById(ISender sender, int id)
    {
        var result = await sender.Send(new GetBookingByIdQuery(id));
        return TypedResults.Ok(result);
    }

    public static async Task<Ok<List<BookingDto>>> GetBranchBookings(ISender sender, [AsParameters] GetBranchBookingsQuery query)
    {
        var result = await sender.Send(query);
        return TypedResults.Ok(result);
    }

    public static async Task<Created<int>> CreateBooking(ISender sender, CreateBookingCommand command)
    {
        var id = await sender.Send(command);
        return TypedResults.Created($"/api/Bookings/{id}", id);
    }

    public static async Task<Ok<List<TimeSlotDto>>> GetAvailableSlots(ISender sender, [AsParameters] GetAvailableSlotsQuery query)
    {
        var result = await sender.Send(query);
        return TypedResults.Ok(result);
    }

    public static async Task<NoContent> ConfirmBooking(ISender sender, int id)
    {
        await sender.Send(new ConfirmBookingCommand(id));
        return TypedResults.NoContent();
    }

    public static async Task<NoContent> CompleteBooking(ISender sender, int id)
    {
        await sender.Send(new CompleteBookingCommand(id));
        return TypedResults.NoContent();
    }

    public static async Task<NoContent> CancelBooking(ISender sender, int id, string? reason)
    {
        await sender.Send(new CancelBookingCommand { Id = id, Reason = reason });
        return TypedResults.NoContent();
    }
}
