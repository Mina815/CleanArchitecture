namespace CleanArchitecture.Application.Bookings.Commands.CreateBooking;

public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidator()
    {
        RuleFor(v => v.CenterId).NotEmpty();
        RuleFor(v => v.BranchId).NotEmpty();
        RuleFor(v => v.ServiceId).NotEmpty();
        RuleFor(v => v.BookingDate)
            .NotEmpty()
            .Must(d => d >= DateOnly.FromDateTime(DateTime.Now))
            .WithMessage("Booking date must be today or in the future.");
        RuleFor(v => v.StartTime)
            .NotEmpty();
    }
}
