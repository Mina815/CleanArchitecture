using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Bookings.EventHandlers;

public class SendReviewRequestNotificationHandler : INotificationHandler<BookingCompletedEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly IInAppNotificationService _notifications;
    private readonly IBookingRealtimeNotifier _realtime;

    public SendReviewRequestNotificationHandler(
        IApplicationDbContext context,
        IInAppNotificationService notifications,
        IBookingRealtimeNotifier realtime)
    {
        _context = context;
        _notifications = notifications;
        _realtime = realtime;
    }

    public async Task Handle(BookingCompletedEvent notification, CancellationToken cancellationToken)
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
            "ReviewRequest",
            "How was your visit?",
            $"Please rate your experience for booking #{booking.Id}.",
            actionUrl: $"/bookings/{booking.Id}/review",
            cancellationToken: cancellationToken);

        await _realtime.BookingCompletedAsync(booking.BranchId, booking.Id, cancellationToken);
    }
}
