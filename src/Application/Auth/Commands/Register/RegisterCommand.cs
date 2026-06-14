namespace CleanArchitecture.Application.Auth.Commands.Register;

public record RegisterCommand : IRequest<AuthResult>
{
    public string Phone { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Email { get; init; }
    public UserRole Role { get; init; } = UserRole.Customer;
}

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResult>
{
    private readonly IAuthService _authService;
    private readonly IJwtService _jwtService;

    public RegisterCommandHandler(IAuthService authService, IJwtService jwtService)
    {
        _authService = authService;
        _jwtService = jwtService;
    }

    public async Task<AuthResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existing = await _authService.FindByPhoneAsync(request.Phone);
        if (existing != null)
            throw new InvalidOperationException("Phone number is already registered.");

        var roleName = request.Role == UserRole.Provider ? Roles.Provider : Roles.Customer;

        var user = await _authService.CreateUserAsync(
            request.Phone, request.Password, request.Name, request.Email, roleName);

        var tokens = await _jwtService.GenerateTokensAsync(user.Id, roleName);

        return new AuthResult
        {
            Token = tokens.accessToken,
            RefreshToken = tokens.refreshToken,
            ExpiresAt = tokens.expiresAt,
            UserId = user.Id,
            Name = user.FullName,
            Role = roleName
        };
    }
}
