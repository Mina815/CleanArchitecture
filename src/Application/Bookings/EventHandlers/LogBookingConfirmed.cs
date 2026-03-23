using CleanArchitecture.Domain.Events;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Bookings.EventHandlers;

public class LogBookingConfirmed : INotificationHandler<BookingConfirmedEvent>
{
    private readonly ILogger<LogBookingConfirmed> _logger;

    public LogBookingConfirmed(ILogger<LogBookingConfirmed> logger)
    {
        _logger = logger;
    }

    public Task Handle(BookingConfirmedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Jamalek domain event: booking {BookingId} confirmed.",
            notification.Booking.Id);
        return Task.CompletedTask;
    }
}
