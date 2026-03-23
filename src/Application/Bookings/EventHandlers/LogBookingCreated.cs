using CleanArchitecture.Domain.Events;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Bookings.EventHandlers;

public class LogBookingCreated : INotificationHandler<BookingCreatedEvent>
{
    private readonly ILogger<LogBookingCreated> _logger;

    public LogBookingCreated(ILogger<LogBookingCreated> logger)
    {
        _logger = logger;
    }

    public Task Handle(BookingCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Jamalek domain event: booking {BookingId} created for branch {BranchId}.",
            notification.Booking.Id,
            notification.Booking.BranchId);
        return Task.CompletedTask;
    }
}
