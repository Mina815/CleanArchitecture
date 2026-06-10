using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Reviews.EventHandlers;

public class UpdateCenterRatingHandler : INotificationHandler<ReviewCreatedEvent>
{
    private readonly IApplicationDbContext _context;

    public UpdateCenterRatingHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(ReviewCreatedEvent notification, CancellationToken cancellationToken)
    {
        var center = await _context.BeautyCenters
            .FirstOrDefaultAsync(c => c.Id == notification.CenterId, cancellationToken);
        if (center is null) return;

        var reviews = await _context.Reviews.AsNoTracking()
            .Where(r => r.CenterId == notification.CenterId && r.IsApproved)
            .ToListAsync(cancellationToken);

        center.TotalReviews = reviews.Count;
        center.AverageRating = reviews.Count > 0 ? (decimal)reviews.Average(r => r.Rating) : 0;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
