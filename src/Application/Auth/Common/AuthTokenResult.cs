namespace CleanArchitecture.Application.Auth.Common;

public record AuthTokenResult(
    string Token,
    DateTimeOffset ExpiresAtUtc,
    string UserId,
    string Phone,
    IReadOnlyCollection<string> Roles);
