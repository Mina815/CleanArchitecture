using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Bookings.EventHandlers;

public class SendBookingCancelledNotificationHandler : INotificationHandler<BookingCancelledEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly IInAppNotificationService _notifications;
    private readonly IBookingRealtimeNotifier _realtime;

    public SendBookingCancelledNotificationHandler(
        IApplicationDbContext context,
        IInAppNotificationService notifications,
        IBookingRealtimeNotifier realtime)
    {
        _context = context;
        _notifications = notifications;
        _realtime = realtime;
    }

    public async Task Handle(BookingCancelledEvent notification, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings
            .AsNoTracking()
            .Where(b => b.Id == notification.Booking.Id)
            .Select(b => new { b.Id, b.BranchId, b.CustomerId, b.Center.OwnerId, b.CancellationReason })
            .FirstOrDefaultAsync(cancellationToken);

        if (booking is null)
        {
            return;
        }

        var message = $"Booking #{booking.Id} has been cancelled.";
        await _notifications.CreateAsync(booking.CustomerId, "BookingCancelled", "Booking cancelled", message, $"/bookings/{booking.Id}", cancellationToken: cancellationToken);
        await _notifications.CreateAsync(booking.OwnerId, "BookingCancelled", "Booking cancelled", message, $"/provider/bookings/{booking.Id}", cancellationToken: cancellationToken);

        await _realtime.BookingCancelledAsync(booking.BranchId, booking.Id, booking.CancellationReason, cancellationToken);
    }
}
