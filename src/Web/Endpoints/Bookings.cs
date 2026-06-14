using CleanArchitecture.Application.Bookings.Commands.CancelBooking;
using CleanArchitecture.Application.Bookings.Commands.CreateBooking;
using CleanArchitecture.Application.Bookings.Queries.GetAvailableSlots;
using CleanArchitecture.Application.Bookings.Queries.GetBranchBookingsToday;
using CleanArchitecture.Application.Bookings.Queries.GetMyBookings;

namespace CleanArchitecture.Web.Endpoints;

public class Bookings : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetMyBookings);
        groupBuilder.MapGet(GetBranchBookingsToday, "branch/{branchId}/today");
        groupBuilder.MapPost(CreateBooking);
        groupBuilder.MapGet(GetAvailableSlots, "slots");
        groupBuilder.MapPost(CancelBooking, "{id}/cancel");
    }

    public static async Task<Ok<List<BookingDto>>> GetMyBookings(ISender sender, bool? upcoming)
    {
        var result = await sender.Send(new GetMyBookingsQuery { Upcoming = upcoming });
        return TypedResults.Ok(result);
    }

    public static async Task<Ok<List<BookingDto>>> GetBranchBookingsToday(ISender sender, int branchId)
    {
        var result = await sender.Send(new GetBranchBookingsTodayQuery(branchId));
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

    public static async Task<NoContent> CancelBooking(ISender sender, int id, string? reason)
    {
        await sender.Send(new CancelBookingCommand { Id = id, Reason = reason });
        return TypedResults.NoContent();
    }
}
