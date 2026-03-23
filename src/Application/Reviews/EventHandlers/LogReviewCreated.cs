using CleanArchitecture.Domain.Events;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Reviews.EventHandlers;

public class LogReviewCreated : INotificationHandler<ReviewCreatedEvent>
{
    private readonly ILogger<LogReviewCreated> _logger;

    public LogReviewCreated(ILogger<LogReviewCreated> logger)
    {
        _logger = logger;
    }

    public Task Handle(ReviewCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Jamalek domain event: review {ReviewId} created for center {CenterId}.",
            notification.Review.Id,
            notification.Review.CenterId);
        return Task.CompletedTask;
    }
}
