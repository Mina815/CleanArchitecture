namespace CleanArchitecture.Application.Auth.Commands.Register;

public record RegisterCommand : IRequest<AuthResult>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public UserRole Role { get; init; } = UserRole.Customer;
}

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResult>
{
    private readonly IAuthService _authService;
    private readonly IJwtService _jwtService;
    private readonly IApplicationDbContext _context;

    public RegisterCommandHandler(IAuthService authService, IJwtService jwtService, IApplicationDbContext context)
    {
        _authService = authService;
        _jwtService = jwtService;
        _context = context;
    }

    public async Task<AuthResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existing = await _authService.FindByEmailAsync(request.Email);
        if (existing != null)
            throw new InvalidOperationException("Email is already registered.");

        var roleName = request.Role == UserRole.Provider ? Roles.Provider : Roles.Customer;

        var user = await _authService.CreateUserAsync(
            request.Email, request.Password, request.Name, request.Phone, roleName);

        if (request.Role == UserRole.Provider)
        {
            _context.BeautyCenters.Add(new BeautyCenter
            {
                OwnerId = user.Id,
                Name = string.Empty,
                NameAr = string.Empty
            });
            await _context.SaveChangesAsync(cancellationToken);
        }

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
