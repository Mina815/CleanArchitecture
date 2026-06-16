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

    public async Task<AuthUserDto?> FindByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
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

    public async Task<AuthUserDto?> FindByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
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

    public async Task<AuthUserDto> CreateUserAsync(string email, string password, string name, string? phone, string role)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            PhoneNumber = phone,
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
