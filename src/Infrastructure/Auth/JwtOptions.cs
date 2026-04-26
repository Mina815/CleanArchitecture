namespace CleanArchitecture.Infrastructure.Auth;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = "Jamalek";

    public string Audience { get; init; } = "JamalekClients";

    public string Key { get; init; } = "CHANGE_ME_WITH_A_LONG_RANDOM_SECRET_KEY_12345";

    public int ExpiryDays { get; init; } = 30;
}
