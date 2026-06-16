namespace CleanArchitecture.Application.Auth.Commands.Login;

public record LoginCommand : IRequest<AuthResult>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResult>
{
    private readonly IAuthService _authService;
    private readonly IJwtService _jwtService;

    public LoginCommandHandler(IAuthService authService, IJwtService jwtService)
    {
        _authService = authService;
        _jwtService = jwtService;
    }

    public async Task<AuthResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _authService.FindByEmailAsync(request.Email);
        if (user == null)
            throw new UnauthorizedAccessException("Invalid email or password.");

        var valid = await _authService.CheckPasswordAsync(user, request.Password);
        if (!valid)
            throw new UnauthorizedAccessException("Invalid email or password.");

        var roles = await _authService.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? Roles.Customer;

        var tokens = await _jwtService.GenerateTokensAsync(user.Id, role);

        return new AuthResult
        {
            Token = tokens.accessToken,
            RefreshToken = tokens.refreshToken,
            ExpiresAt = tokens.expiresAt,
            UserId = user.Id,
            Name = user.FullName,
            Role = role
        };
    }
}
