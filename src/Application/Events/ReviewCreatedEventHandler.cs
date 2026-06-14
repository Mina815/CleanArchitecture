using CleanArchitecture.Domain.Events;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Events;

public class ReviewCreatedEventHandler : INotificationHandler<ReviewCreatedEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<ReviewCreatedEventHandler> _logger;

    public ReviewCreatedEventHandler(IApplicationDbContext context, ILogger<ReviewCreatedEventHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(ReviewCreatedEvent notification, CancellationToken cancellationToken)
    {
        var review = notification.Review;

        var ratings = await _context.Reviews
            .Where(r => r.CenterId == review.CenterId && r.IsApproved)
            .Select(r => (double)r.Rating)
            .ToListAsync(cancellationToken);

        var center = await _context.BeautyCenters
            .FindAsync([review.CenterId], cancellationToken);

        if (center is not null)
        {
            center.TotalReviews = ratings.Count;
            center.AverageRating = ratings.Count > 0 ? ratings.Average() : 0;

            await _context.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation(
            "ReviewCreatedEvent handled for center {CenterId}. New average rating: {Rating}",
            review.CenterId, center?.AverageRating);
    }
}
