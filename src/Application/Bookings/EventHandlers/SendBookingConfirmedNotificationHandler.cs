using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Bookings.EventHandlers;

public class SendBookingConfirmedNotificationHandler : INotificationHandler<BookingConfirmedEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly IInAppNotificationService _notifications;
    private readonly IBookingRealtimeNotifier _realtime;

    public SendBookingConfirmedNotificationHandler(
        IApplicationDbContext context,
        IInAppNotificationService notifications,
        IBookingRealtimeNotifier realtime)
    {
        _context = context;
        _notifications = notifications;
        _realtime = realtime;
    }

    public async Task Handle(BookingConfirmedEvent notification, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings
            .AsNoTracking()
            .Where(b => b.Id == notification.Booking.Id)
            .Select(b => new { b.Id, b.BranchId, b.CustomerId })
            .FirstOrDefaultAsync(cancellationToken);

        if (booking is null)
        {
            return;
        }

        await _notifications.CreateAsync(
            booking.CustomerId,
            "BookingConfirmed",
            "Booking confirmed",
            $"Your booking #{booking.Id} has been confirmed.",
            actionUrl: $"/bookings/{booking.Id}",
            cancellationToken: cancellationToken);

        await _realtime.BookingConfirmedAsync(booking.BranchId, booking.Id, cancellationToken);
    }
}
