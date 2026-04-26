using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Web.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CleanArchitecture.Web.Services;

public sealed class BookingRealtimeNotifier : IBookingRealtimeNotifier
{
    private readonly IHubContext<BookingHub> _hubContext;

    public BookingRealtimeNotifier(IHubContext<BookingHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task NewBookingAsync(int branchId, int bookingId, CancellationToken cancellationToken = default)
    {
        return _hubContext.Clients.Group(BookingHub.GetBranchGroup(branchId))
            .SendAsync("NewBooking", new { bookingId }, cancellationToken);
    }

    public Task BookingConfirmedAsync(int branchId, int bookingId, CancellationToken cancellationToken = default)
    {
        return _hubContext.Clients.Group(BookingHub.GetBranchGroup(branchId))
            .SendAsync("BookingConfirmed", bookingId, cancellationToken);
    }

    public Task BookingCancelledAsync(int branchId, int bookingId, string? reason, CancellationToken cancellationToken = default)
    {
        return _hubContext.Clients.Group(BookingHub.GetBranchGroup(branchId))
            .SendAsync("BookingCancelled", bookingId, reason, cancellationToken);
    }

    public Task BookingCompletedAsync(int branchId, int bookingId, CancellationToken cancellationToken = default)
    {
        return _hubContext.Clients.Group(BookingHub.GetBranchGroup(branchId))
            .SendAsync("BookingCompleted", bookingId, cancellationToken);
    }
}
