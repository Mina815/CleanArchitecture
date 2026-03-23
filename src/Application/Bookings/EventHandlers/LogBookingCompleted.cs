using CleanArchitecture.Domain.Events;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Bookings.EventHandlers;

public class LogBookingCompleted : INotificationHandler<BookingCompletedEvent>
{
    private readonly ILogger<LogBookingCompleted> _logger;

    public LogBookingCompleted(ILogger<LogBookingCompleted> logger)
    {
        _logger = logger;
    }

    public Task Handle(BookingCompletedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Jamalek domain event: booking {BookingId} completed.",
            notification.Booking.Id);
        return Task.CompletedTask;
    }
}
