using CleanArchitecture.Application.Auth.Common;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Constants;
using CleanArchitecture.Infrastructure.Identity;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Infrastructure.Auth;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(UserManager<ApplicationUser> userManager, IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthTokenResult> RegisterAsync(
        string phone,
        string name,
        string? email,
        string password,
        string role,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var normalizedRole = string.Equals(role, Roles.Provider, StringComparison.OrdinalIgnoreCase)
            ? Roles.Provider
            : Roles.Customer;

        var user = new ApplicationUser
        {
            UserName = phone,
            PhoneNumber = phone,
            Email = email,
            FullName = name,
            IsActive = true
        };

        var createResult = await _userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
        {
            var failures = createResult.Errors.Select(e => new ValidationFailure(e.Code, e.Description));
            throw new ValidationException(failures);
        }

        await _userManager.AddToRoleAsync(user, normalizedRole);
        var token = await _jwtTokenService.GenerateTokenAsync(user.Id, user.PhoneNumber!, [normalizedRole]);

        return new AuthTokenResult(token.Token, token.ExpiresAtUtc, user.Id, user.PhoneNumber!, [normalizedRole]);
    }

    public async Task<AuthTokenResult?> LoginAsync(
        string phone,
        string password,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await _userManager.FindByNameAsync(phone);
        if (user is null || !user.IsActive)
        {
            return null;
        }

        var ok = await _userManager.CheckPasswordAsync(user, password);
        if (!ok)
        {
            return null;
        }

        var roles = await _userManager.GetRolesAsync(user);
        var rolesArray = roles.ToArray();
        var token = await _jwtTokenService.GenerateTokenAsync(user.Id, user.PhoneNumber ?? phone, rolesArray);

        return new AuthTokenResult(token.Token, token.ExpiresAtUtc, user.Id, user.PhoneNumber ?? phone, rolesArray);
    }
}
