namespace CleanArchitecture.Application.Auth.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(v => v.Phone).NotEmpty().MaximumLength(32);
        RuleFor(v => v.Password).NotEmpty();
    }
}
