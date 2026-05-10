using CleanArchitecture.Application.Auth.Commands.Login;
using CleanArchitecture.Application.Auth.Commands.Register;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CleanArchitecture.Web.Endpoints;

public class Auth : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(Register, "register");
        groupBuilder.MapPost(Login, "login");
    }

    [EndpointSummary("Register user account")]
    public static async Task<Ok<AuthTokenResponse>> Register(
        RegisterCommand command,
        ISender sender)
    {
        var result = await sender.Send(command);
        return TypedResults.Ok(new AuthTokenResponse(result.Token, result.ExpiresAtUtc, result.UserId, result.Phone, result.Roles));
    }

    [EndpointSummary("Login with phone and password")]
    public static async Task<Results<Ok<AuthTokenResponse>, UnauthorizedHttpResult>> Login(
        LoginCommand command,
        ISender sender)
    {
        var result = await sender.Send(command);
        if (result is null)
        {
            return TypedResults.Unauthorized();
        }

        return TypedResults.Ok(new AuthTokenResponse(result.Token, result.ExpiresAtUtc, result.UserId, result.Phone, result.Roles));
    }
}

public record AuthTokenResponse(string Token, DateTimeOffset ExpiresAtUtc, string UserId, string Phone, IReadOnlyCollection<string> Roles);
