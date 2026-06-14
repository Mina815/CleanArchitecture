using CleanArchitecture.Domain.Events;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Events;

public class BookingCreatedEventHandler : INotificationHandler<BookingCreatedEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly INotificationService _notificationService;
    private readonly IBookingHubService _hubService;
    private readonly ILogger<BookingCreatedEventHandler> _logger;

    public BookingCreatedEventHandler(
        IApplicationDbContext context,
        INotificationService notificationService,
        IBookingHubService hubService,
        ILogger<BookingCreatedEventHandler> logger)
    {
        _context = context;
        _notificationService = notificationService;
        _hubService = hubService;
        _logger = logger;
    }

    public async Task Handle(BookingCreatedEvent notification, CancellationToken cancellationToken)
    {
        var booking = notification.Booking;

        var center = await _context.BeautyCenters
            .FindAsync([booking.CenterId], cancellationToken);

        var notificationEntity = new Domain.Entities.Notification
        {
            UserId = center!.OwnerId,
            Type = NotificationType.BookingCreated,
            Title = "New Booking",
            Message = $"New booking for {booking.BookingDate} at {booking.StartTime:hh\\:mm}",
            ActionUrl = $"/bookings/{booking.Id}",
            Data = System.Text.Json.JsonSerializer.Serialize(new { booking.Id, booking.BranchId })
        };

        _context.Notifications.Add(notificationEntity);

        await _notificationService.SendPushNotificationAsync(
            center.OwnerId,
            "New Booking",
            $"A new booking has been made for {booking.BookingDate:yyyy-MM-dd}.");

        await _hubService.NotifyNewBookingAsync(booking.BranchId, new
        {
            booking.Id,
            booking.BookingDate,
            booking.StartTime,
            booking.EndTime,
            booking.CustomerNotes
        });

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("BookingCreatedEvent handled for booking {BookingId}", booking.Id);
    }
}
