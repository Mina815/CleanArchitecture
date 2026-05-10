using CleanArchitecture.Domain.Constants;

namespace CleanArchitecture.Application.Auth.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(v => v.Phone).NotEmpty().MaximumLength(32);
        RuleFor(v => v.Name).NotEmpty().MaximumLength(100);
        RuleFor(v => v.Email).EmailAddress().When(v => !string.IsNullOrWhiteSpace(v.Email));
        RuleFor(v => v.Password).NotEmpty().MinimumLength(6);
        RuleFor(v => v.Role)
            .NotEmpty()
            .Must(role =>
                role.Equals(Roles.Provider, StringComparison.OrdinalIgnoreCase) ||
                role.Equals(Roles.Customer, StringComparison.OrdinalIgnoreCase))
            .WithMessage($"Role must be '{Roles.Customer}' or '{Roles.Provider}'.");
    }
}
