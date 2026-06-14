using CleanArchitecture.Domain.Events;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Events;

public class BookingCancelledEventHandler : INotificationHandler<BookingCancelledEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly INotificationService _notificationService;
    private readonly IBookingHubService _hubService;
    private readonly ILogger<BookingCancelledEventHandler> _logger;

    public BookingCancelledEventHandler(
        IApplicationDbContext context,
        INotificationService notificationService,
        IBookingHubService hubService,
        ILogger<BookingCancelledEventHandler> logger)
    {
        _context = context;
        _notificationService = notificationService;
        _hubService = hubService;
        _logger = logger;
    }

    public async Task Handle(BookingCancelledEvent notification, CancellationToken cancellationToken)
    {
        var booking = notification.Booking;

        var center = await _context.BeautyCenters
            .FindAsync([booking.CenterId], cancellationToken);

        var message = $"Booking on {booking.BookingDate} at {booking.StartTime:hh\\:mm} has been cancelled.";

        var customerNotification = new Domain.Entities.Notification
        {
            UserId = booking.CustomerId,
            Type = NotificationType.BookingCancelled,
            Title = "Booking Cancelled",
            Message = message,
            ActionUrl = $"/bookings/{booking.Id}",
            Data = System.Text.Json.JsonSerializer.Serialize(new { booking.Id, booking.CancellationReason })
        };

        _context.Notifications.Add(customerNotification);

        var providerNotification = new Domain.Entities.Notification
        {
            UserId = center!.OwnerId,
            Type = NotificationType.BookingCancelled,
            Title = "Booking Cancelled",
            Message = message,
            ActionUrl = $"/bookings/{booking.Id}",
            Data = System.Text.Json.JsonSerializer.Serialize(new { booking.Id, booking.CancellationReason })
        };

        _context.Notifications.Add(providerNotification);

        await _notificationService.SendPushNotificationAsync(
            booking.CustomerId, "Booking Cancelled", message);

        await _hubService.NotifyBookingCancelledAsync(booking.BranchId, booking.Id, booking.CancellationReason);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("BookingCancelledEvent handled for booking {BookingId}", booking.Id);
    }
}
