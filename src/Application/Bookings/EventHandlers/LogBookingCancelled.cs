using CleanArchitecture.Domain.Events;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Bookings.EventHandlers;

public class LogBookingCancelled : INotificationHandler<BookingCancelledEvent>
{
    private readonly ILogger<LogBookingCancelled> _logger;

    public LogBookingCancelled(ILogger<LogBookingCancelled> logger)
    {
        _logger = logger;
    }

    public Task Handle(BookingCancelledEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Jamalek domain event: booking {BookingId} cancelled.",
            notification.Booking.Id);
        return Task.CompletedTask;
    }
}
