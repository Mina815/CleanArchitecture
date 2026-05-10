using CleanArchitecture.Application.Auth.Common;

namespace CleanArchitecture.Application.Common.Interfaces;

public interface IAuthService
{
    Task<AuthTokenResult> RegisterAsync(
        string phone,
        string name,
        string? email,
        string password,
        string role,
        CancellationToken cancellationToken = default);

    Task<AuthTokenResult?> LoginAsync(
        string phone,
        string password,
        CancellationToken cancellationToken = default);
}
