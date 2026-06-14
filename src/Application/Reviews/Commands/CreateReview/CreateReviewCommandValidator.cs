namespace CleanArchitecture.Application.Reviews.Commands.CreateReview;

public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewCommandValidator()
    {
        RuleFor(v => v.CenterId).NotEmpty();
        RuleFor(v => v.BookingId).NotEmpty();
        RuleFor(v => v.Rating)
            .InclusiveBetween(1, 5)
            .WithMessage("Rating must be between 1 and 5.");
        RuleFor(v => v.Comment)
            .MaximumLength(2000);
    }
}
