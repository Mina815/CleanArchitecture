using CleanArchitecture.Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Infrastructure.Identity;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<AuthUserDto?> FindByPhoneAsync(string phone)
    {
        var user = await _userManager.FindByNameAsync(phone);
        if (user == null) return null;

        return new AuthUserDto
        {
            Id = user.Id,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email,
            FullName = user.FullName,
            FullNameAr = user.FullNameAr
        };
    }

    public async Task<bool> CheckPasswordAsync(AuthUserDto user, string password)
    {
        var identityUser = await _userManager.FindByIdAsync(user.Id);
        if (identityUser == null) return false;
        return await _userManager.CheckPasswordAsync(identityUser, password);
    }

    public async Task<IList<string>> GetRolesAsync(AuthUserDto user)
    {
        var identityUser = await _userManager.FindByIdAsync(user.Id);
        if (identityUser == null) return new List<string>();
        return await _userManager.GetRolesAsync(identityUser);
    }

    public async Task<AuthUserDto> CreateUserAsync(string phone, string password, string name, string? email, string role)
    {
        var user = new ApplicationUser
        {
            UserName = phone,
            PhoneNumber = phone,
            Email = email,
            FullName = name,
            FullNameAr = name
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            throw new InvalidOperationException($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");

        if (!await _roleManager.RoleExistsAsync(role))
            await _roleManager.CreateAsync(new IdentityRole(role));

        await _userManager.AddToRoleAsync(user, role);

        return new AuthUserDto
        {
            Id = user.Id,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email,
            FullName = user.FullName,
            FullNameAr = user.FullNameAr
        };
    }
}
