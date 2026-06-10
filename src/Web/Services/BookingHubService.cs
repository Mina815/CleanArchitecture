using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Web.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CleanArchitecture.Web.Services;

public class BookingHubService : IBookingHubService
{
    private readonly IHubContext<BookingHub> _hubContext;

    public BookingHubService(IHubContext<BookingHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task NotifyNewBookingAsync(int branchId, object bookingData, CancellationToken cancellationToken = default)
        => _hubContext.Clients.Group(BookingHub.BranchRoom(branchId)).SendAsync("NewBooking", bookingData, cancellationToken);

    public Task NotifyBookingConfirmedAsync(int branchId, int bookingId, CancellationToken cancellationToken = default)
        => _hubContext.Clients.Group(BookingHub.BranchRoom(branchId)).SendAsync("BookingConfirmed", bookingId, cancellationToken);

    public Task NotifyBookingCancelledAsync(int branchId, int bookingId, string? reason, CancellationToken cancellationToken = default)
        => _hubContext.Clients.Group(BookingHub.BranchRoom(branchId)).SendAsync("BookingCancelled", bookingId, reason, cancellationToken);

    public Task NotifyBookingCompletedAsync(int branchId, int bookingId, CancellationToken cancellationToken = default)
        => _hubContext.Clients.Group(BookingHub.BranchRoom(branchId)).SendAsync("BookingCompleted", bookingId, cancellationToken);
}
