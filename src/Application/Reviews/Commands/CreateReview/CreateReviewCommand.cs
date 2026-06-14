namespace CleanArchitecture.Application.Reviews.Commands.CreateReview;

public record CreateReviewCommand : IRequest<int>
{
    public int CenterId { get; init; }
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
            .FindAsync([request.BookingId], cancellationToken);
        Guard.Against.NotFound(request.BookingId, booking);

        if (booking!.Status != BookingStatus.Completed)
            throw new InvalidOperationException("Can only review completed bookings.");

        if (booking.CustomerId != _user.Id)
            throw new ForbiddenAccessException();

        var existingReview = await _context.Reviews
            .AnyAsync(r => r.BookingId == request.BookingId, cancellationToken);

        if (existingReview)
            throw new InvalidOperationException("You have already reviewed this booking.");

        var review = new Review
        {
            CustomerId = _user.Id!,
            CenterId = request.CenterId,
            BookingId = request.BookingId,
            Rating = request.Rating,
            Comment = request.Comment
        };

        review.AddDomainEvent(new ReviewCreatedEvent(review));

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync(cancellationToken);

        return review.Id;
    }
}
