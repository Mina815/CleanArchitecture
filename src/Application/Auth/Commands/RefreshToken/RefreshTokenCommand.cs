namespace CleanArchitecture.Application.Auth.Commands.RefreshToken;

public record RefreshTokenCommand : IRequest<TokenResult>
{
    public string UserId { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
}

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, TokenResult>
{
    private readonly IJwtService _jwtService;
    private readonly IAuthService _authService;

    public RefreshTokenCommandHandler(IJwtService jwtService, IAuthService authService)
    {
        _jwtService = jwtService;
        _authService = authService;
    }

    public async Task<TokenResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var userId = await _jwtService.ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);
        if (userId == null)
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        var user = await _authService.FindByPhoneAsync(request.UserId);
        var roles = user != null ? await _authService.GetRolesAsync(user) : new List<string>();
        var role = roles.FirstOrDefault() ?? Roles.Customer;

        var tokens = await _jwtService.GenerateTokensAsync(userId, role);

        return new TokenResult
        {
            Token = tokens.accessToken,
            RefreshToken = tokens.refreshToken,
            ExpiresAt = tokens.expiresAt
        };
    }
}
