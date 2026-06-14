namespace CleanArchitecture.Application.Auth.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(v => v.Phone).NotEmpty();
        RuleFor(v => v.Password).NotEmpty();
    }
}
