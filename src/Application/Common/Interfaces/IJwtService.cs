namespace CleanArchitecture.Application.Common.Interfaces;

public interface IJwtService
{
    Task<(string accessToken, string refreshToken, DateTime expiresAt)> GenerateTokensAsync(string userId, string role);
    Task<string?> ValidateRefreshTokenAsync(string userId, string refreshToken);
}
