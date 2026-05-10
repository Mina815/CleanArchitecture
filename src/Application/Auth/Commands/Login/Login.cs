using CleanArchitecture.Application.Auth.Common;
using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.Application.Auth.Commands.Login;

public record LoginCommand : IRequest<AuthTokenResult?>
{
    public string Phone { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthTokenResult?>
{
    private readonly IAuthService _authService;

    public LoginCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public Task<AuthTokenResult?> Handle(LoginCommand request, CancellationToken cancellationToken) =>
        _authService.LoginAsync(request.Phone, request.Password, cancellationToken);
}
