using CleanArchitecture.Domain.Events;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Events;

public class BookingConfirmedEventHandler : INotificationHandler<BookingConfirmedEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly INotificationService _notificationService;
    private readonly IBookingHubService _hubService;
    private readonly ILogger<BookingConfirmedEventHandler> _logger;

    public BookingConfirmedEventHandler(
        IApplicationDbContext context,
        INotificationService notificationService,
        IBookingHubService hubService,
        ILogger<BookingConfirmedEventHandler> logger)
    {
        _context = context;
        _notificationService = notificationService;
        _hubService = hubService;
        _logger = logger;
    }

    public async Task Handle(BookingConfirmedEvent notification, CancellationToken cancellationToken)
    {
        var booking = notification.Booking;

        var notificationEntity = new Domain.Entities.Notification
        {
            UserId = booking.CustomerId,
            Type = NotificationType.BookingConfirmed,
            Title = "Booking Confirmed",
            Message = $"Your booking on {booking.BookingDate} at {booking.StartTime:hh\\:mm} has been confirmed.",
            ActionUrl = $"/bookings/{booking.Id}",
            Data = System.Text.Json.JsonSerializer.Serialize(new { booking.Id })
        };

        _context.Notifications.Add(notificationEntity);

        await _notificationService.SendPushNotificationAsync(
            booking.CustomerId,
            "Booking Confirmed",
            $"Your booking on {booking.BookingDate} has been confirmed!");

        await _hubService.NotifyBookingConfirmedAsync(booking.BranchId, booking.Id);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("BookingConfirmedEvent handled for booking {BookingId}", booking.Id);
    }
}
