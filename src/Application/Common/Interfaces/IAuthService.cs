namespace CleanArchitecture.Application.Common.Interfaces;

public class AuthUserDto
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? FullNameAr { get; set; }
}

public interface IAuthService
{
    Task<AuthUserDto?> FindByPhoneAsync(string phone);
    Task<bool> CheckPasswordAsync(AuthUserDto user, string password);
    Task<IList<string>> GetRolesAsync(AuthUserDto user);
    Task<AuthUserDto> CreateUserAsync(string phone, string password, string name, string? email, string role);
}
