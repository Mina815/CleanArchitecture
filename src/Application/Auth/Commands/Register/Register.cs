using CleanArchitecture.Application.Auth.Common;
using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.Application.Auth.Commands.Register;

public record RegisterCommand : IRequest<AuthTokenResult>
{
    public string Phone { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string Password { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
}

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthTokenResult>
{
    private readonly IAuthService _authService;

    public RegisterCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public Task<AuthTokenResult> Handle(RegisterCommand request, CancellationToken cancellationToken) =>
        _authService.RegisterAsync(request.Phone, request.Name, request.Email, request.Password, request.Role, cancellationToken);
}
