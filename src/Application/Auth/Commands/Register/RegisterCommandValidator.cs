namespace CleanArchitecture.Application.Auth.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(v => v.Phone)
            .NotEmpty()
            .Matches(@"^01[0-9]{9}$")
            .WithMessage("Phone must be a valid Egyptian phone number (e.g. 01012345678).");

        RuleFor(v => v.Password)
            .NotEmpty()
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters.");

        RuleFor(v => v.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(v => v.Role)
            .Must(r => r is UserRole.Customer or UserRole.Provider)
            .WithMessage("Role must be Customer or Provider.");
    }
}
