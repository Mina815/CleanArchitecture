using FluentValidation;

namespace CleanArchitecture.Application.Bookings.Commands.CreateBooking;

public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidator()
    {
        RuleFor(v => v.CustomerNotes).MaximumLength(1000);
    }
}
