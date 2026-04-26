using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Constants;
using CleanArchitecture.Infrastructure.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Web.Endpoints;

public class Auth : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(Register, "register");
        groupBuilder.MapPost(Login, "login");
    }

    [EndpointSummary("Register user account")]
    public static async Task<Results<Ok<AuthTokenResponse>, ValidationProblem>> Register(
        RegisterRequest request,
        UserManager<ApplicationUser> userManager,
        IJwtTokenService jwtTokenService)
    {
        var role = string.Equals(request.Role, Roles.Provider, StringComparison.OrdinalIgnoreCase)
            ? Roles.Provider
            : Roles.Customer;

        var user = new ApplicationUser
        {
            UserName = request.Phone,
            PhoneNumber = request.Phone,
            Email = request.Email,
            FullName = request.Name,
            IsActive = true
        };

        var createResult = await userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            return TypedResults.ValidationProblem(createResult.Errors
                .GroupBy(e => e.Code)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray()));
        }

        await userManager.AddToRoleAsync(user, role);
        var token = await jwtTokenService.GenerateTokenAsync(user.Id, user.PhoneNumber!, [role]);
        return TypedResults.Ok(new AuthTokenResponse(token.Token, token.ExpiresAtUtc, user.Id, user.PhoneNumber!, [role]));
    }

    [EndpointSummary("Login with phone and password")]
    public static async Task<Results<Ok<AuthTokenResponse>, UnauthorizedHttpResult>> Login(
        LoginRequest request,
        UserManager<ApplicationUser> userManager,
        IJwtTokenService jwtTokenService)
    {
        var user = await userManager.FindByNameAsync(request.Phone);
        if (user is null || !user.IsActive)
        {
            return TypedResults.Unauthorized();
        }

        var ok = await userManager.CheckPasswordAsync(user, request.Password);
        if (!ok)
        {
            return TypedResults.Unauthorized();
        }

        var roles = await userManager.GetRolesAsync(user);
        var rolesArray = roles.ToArray();
        var token = await jwtTokenService.GenerateTokenAsync(user.Id, user.PhoneNumber ?? request.Phone, rolesArray);
        return TypedResults.Ok(new AuthTokenResponse(token.Token, token.ExpiresAtUtc, user.Id, user.PhoneNumber ?? request.Phone, rolesArray));
    }
}

public record RegisterRequest(string Phone, string Name, string? Email, string Password, string Role);

public record LoginRequest(string Phone, string Password);

public record AuthTokenResponse(string Token, DateTimeOffset ExpiresAtUtc, string UserId, string Phone, IReadOnlyCollection<string> Roles);
