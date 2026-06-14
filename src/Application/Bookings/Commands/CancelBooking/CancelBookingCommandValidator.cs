namespace CleanArchitecture.Application.Bookings.Commands.CancelBooking;

public class CancelBookingCommandValidator : AbstractValidator<CancelBookingCommand>
{
    public CancelBookingCommandValidator()
    {
        RuleFor(v => v.Id).NotEmpty();
    }
}
