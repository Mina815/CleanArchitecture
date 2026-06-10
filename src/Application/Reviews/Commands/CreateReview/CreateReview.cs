using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using CleanArchitecture.Domain.Constants;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Reviews.Commands.CreateReview;

[Authorize(Roles = Roles.Customer)]
public record CreateReviewCommand : IRequest<int>
{
    public int BookingId { get; init; }
    public int Rating { get; init; }
    public string? Comment { get; init; }
}

public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public CreateReviewCommandHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<int> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == request.BookingId && b.CustomerId == _user.Id, cancellationToken);
        Guard.Against.NotFound(request.BookingId, booking);

        if (booking.Status != BookingStatus.Completed)
            throw new InvalidOperationException("Only completed bookings can be reviewed.");

        var exists = await _context.Reviews.AnyAsync(r => r.BookingId == request.BookingId, cancellationToken);
        if (exists) throw new InvalidOperationException("Review already exists for this booking.");

        var review = Review.Create(_user.Id!, booking.CenterId, request.BookingId, request.Rating, request.Comment);
        _context.Reviews.Add(review);
        await _context.SaveChangesAsync(cancellationToken);
        return review.Id;
    }
}
