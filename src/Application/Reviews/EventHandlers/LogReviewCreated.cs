using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Reviews.EventHandlers;

public class LogReviewCreated : INotificationHandler<ReviewCreatedEvent>
{
    private readonly IApplicationDbContext _context;

    public LogReviewCreated(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ReviewCreatedEvent notification, CancellationToken cancellationToken)
    {
        var centerId = notification.Review.CenterId;
        var approvedReviews = _context.Reviews
            .Where(r => r.CenterId == centerId && r.IsApproved);

        var total = await approvedReviews.CountAsync(cancellationToken);
        var average = total == 0
            ? 0m
            : Convert.ToDecimal(await approvedReviews.AverageAsync(r => r.Rating, cancellationToken));

        var center = await _context.BeautyCenters.FirstOrDefaultAsync(c => c.Id == centerId, cancellationToken);
        if (center is null)
        {
            return;
        }

        center.TotalReviews = total;
        center.AverageRating = Math.Round(average, 2, MidpointRounding.AwayFromZero);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
