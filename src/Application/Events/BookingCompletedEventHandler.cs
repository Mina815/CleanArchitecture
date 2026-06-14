using CleanArchitecture.Domain.Events;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Events;

public class BookingCompletedEventHandler : INotificationHandler<BookingCompletedEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly INotificationService _notificationService;
    private readonly IBookingHubService _hubService;
    private readonly ILogger<BookingCompletedEventHandler> _logger;

    public BookingCompletedEventHandler(
        IApplicationDbContext context,
        INotificationService notificationService,
        IBookingHubService hubService,
        ILogger<BookingCompletedEventHandler> logger)
    {
        _context = context;
        _notificationService = notificationService;
        _hubService = hubService;
        _logger = logger;
    }

    public async Task Handle(BookingCompletedEvent notification, CancellationToken cancellationToken)
    {
        var booking = notification.Booking;

        var reviewNotification = new Domain.Entities.Notification
        {
            UserId = booking.CustomerId,
            Type = NotificationType.ReviewRequest,
            Title = "How was your experience?",
            Message = $"Please leave a review for your visit on {booking.BookingDate}.",
            ActionUrl = $"/reviews?bookingId={booking.Id}",
            Data = System.Text.Json.JsonSerializer.Serialize(new { booking.Id, booking.CenterId })
        };

        _context.Notifications.Add(reviewNotification);

        await _notificationService.SendPushNotificationAsync(
            booking.CustomerId,
            "How was your experience?",
            "Please leave a review for your recent visit.");

        await _hubService.NotifyBookingCompletedAsync(booking.BranchId, booking.Id);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("BookingCompletedEvent handled for booking {BookingId}", booking.Id);
    }
}
