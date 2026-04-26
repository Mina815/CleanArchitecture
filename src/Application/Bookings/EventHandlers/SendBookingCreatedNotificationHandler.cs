using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Bookings.EventHandlers;

public class SendBookingCreatedNotificationHandler : INotificationHandler<BookingCreatedEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly IInAppNotificationService _notifications;
    private readonly IBookingRealtimeNotifier _realtime;

    public SendBookingCreatedNotificationHandler(
        IApplicationDbContext context,
        IInAppNotificationService notifications,
        IBookingRealtimeNotifier realtime)
    {
        _context = context;
        _notifications = notifications;
        _realtime = realtime;
    }

    public async Task Handle(BookingCreatedEvent notification, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings
            .AsNoTracking()
            .Where(b => b.Id == notification.Booking.Id)
            .Select(b => new { b.Id, b.BranchId, b.Center.OwnerId, b.CustomerId })
            .FirstOrDefaultAsync(cancellationToken);

        if (booking is null)
        {
            return;
        }

        await _notifications.CreateAsync(
            booking.OwnerId,
            "BookingCreated",
            "New booking received",
            $"Booking #{booking.Id} is pending confirmation.",
            actionUrl: $"/provider/bookings/{booking.Id}",
            cancellationToken: cancellationToken);

        await _notifications.CreateAsync(
            booking.CustomerId,
            "BookingCreated",
            "Booking submitted",
            $"Your booking #{booking.Id} is pending confirmation.",
            actionUrl: $"/bookings/{booking.Id}",
            cancellationToken: cancellationToken);

        await _realtime.NewBookingAsync(booking.BranchId, booking.Id, cancellationToken);
    }
}
