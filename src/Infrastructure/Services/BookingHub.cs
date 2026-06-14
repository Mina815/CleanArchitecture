using CleanArchitecture.Application.Common.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace CleanArchitecture.Infrastructure.Services;

public class BookingHub : Hub
{
    public async Task JoinBranchGroup(int branchId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"branch_{branchId}");
    }

    public async Task LeaveBranchGroup(int branchId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"branch_{branchId}");
    }
}

public class BookingHubService : IBookingHubService
{
    private readonly IHubContext<BookingHub> _hubContext;

    public BookingHubService(IHubContext<BookingHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyNewBookingAsync(int branchId, object bookingData)
    {
        await _hubContext.Clients.Group($"branch_{branchId}").SendAsync("NewBooking", bookingData);
    }

    public async Task NotifyBookingConfirmedAsync(int branchId, int bookingId)
    {
        await _hubContext.Clients.Group($"branch_{branchId}").SendAsync("BookingConfirmed", bookingId);
    }

    public async Task NotifyBookingCancelledAsync(int branchId, int bookingId, string? reason)
    {
        await _hubContext.Clients.Group($"branch_{branchId}").SendAsync("BookingCancelled", new { bookingId, reason });
    }

    public async Task NotifyBookingCompletedAsync(int branchId, int bookingId)
    {
        await _hubContext.Clients.Group($"branch_{branchId}").SendAsync("BookingCompleted", bookingId);
    }
}
