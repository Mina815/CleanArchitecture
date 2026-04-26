namespace CleanArchitecture.Application.Common.Interfaces;

public interface IJwtTokenService
{
    Task<(string Token, DateTimeOffset ExpiresAtUtc)> GenerateTokenAsync(
        string userId,
        string phone,
        IReadOnlyCollection<string> roles);
}
