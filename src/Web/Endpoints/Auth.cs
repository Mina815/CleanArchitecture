using CleanArchitecture.Domain.Constants;
using CleanArchitecture.Domain.Enums;
using CleanArchitecture.Infrastructure.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Web.Endpoints;

public class Auth : IEndpointGroup
{
    public static string? RoutePrefix => "/api/auth";

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(Register, "register");
        groupBuilder.MapPost(Login, "login");
        groupBuilder.MapPost(UpdateFcmToken, "update-fcm-token").RequireAuthorization();
    }

    public record RegisterRequest(string Phone, string Password, string Name, string? Email, UserRole Role);
    public record LoginRequest(string Phone, string Password);
    public record AuthResponse(string Token, string UserId, string Name, string Role);
    public record UpdateFcmTokenRequest(string FcmToken);

    [EndpointSummary("Register new user")]
    public static async Task<Results<Ok<AuthResponse>, BadRequest<string>>> Register(
        UserManager<ApplicationUser> userManager, JwtTokenService jwtTokenService, RegisterRequest request)
    {
        var existing = await userManager.FindByNameAsync(request.Phone);
        if (existing is not null) return TypedResults.BadRequest("Phone number already registered.");

        var user = new ApplicationUser
        {
            UserName = request.Phone,
            PhoneNumber = request.Phone,
            Email = request.Email ?? $"{request.Phone}@jamalek.local",
            Name = request.Name,
            Role = request.Role,
            IsActive = true
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return TypedResults.BadRequest(string.Join(", ", result.Errors.Select(e => e.Description)));

        var roleName = request.Role switch
        {
            UserRole.Provider => Roles.Provider,
            UserRole.Admin => Roles.Administrator,
            _ => Roles.Customer
        };
        await userManager.AddToRoleAsync(user, roleName);

        var roles = await userManager.GetRolesAsync(user);
        var token = jwtTokenService.GenerateToken(user, roles);
        return TypedResults.Ok(new AuthResponse(token, user.Id, user.Name ?? user.PhoneNumber!, user.Role.ToString()));
    }

    [EndpointSummary("Login and get JWT token")]
    public static async Task<Results<Ok<AuthResponse>, UnauthorizedHttpResult>> Login(
        UserManager<ApplicationUser> userManager, JwtTokenService jwtTokenService, LoginRequest request)
    {
        var user = await userManager.FindByNameAsync(request.Phone);
        if (user is null || !user.IsActive) return TypedResults.Unauthorized();

        var valid = await userManager.CheckPasswordAsync(user, request.Password);
        if (!valid) return TypedResults.Unauthorized();

        var roles = await userManager.GetRolesAsync(user);
        var token = jwtTokenService.GenerateToken(user, roles);
        return TypedResults.Ok(new AuthResponse(token, user.Id, user.Name ?? user.PhoneNumber!, user.Role.ToString()));
    }

    [EndpointSummary("Update FCM token for push notifications")]
    public static async Task<Results<NoContent, UnauthorizedHttpResult>> UpdateFcmToken(
        UserManager<ApplicationUser> userManager, HttpContext httpContext, UpdateFcmTokenRequest request)
    {
        var userId = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId is null) return TypedResults.Unauthorized();

        var user = await userManager.FindByIdAsync(userId);
        if (user is null) return TypedResults.Unauthorized();

        user.FcmToken = request.FcmToken;
        await userManager.UpdateAsync(user);
        return TypedResults.NoContent();
    }
}
