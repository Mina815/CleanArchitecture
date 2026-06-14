using CleanArchitecture.Application.Auth.Commands.Login;
using CleanArchitecture.Application.Auth.Commands.RefreshToken;
using CleanArchitecture.Application.Auth.Commands.Register;

namespace CleanArchitecture.Web.Endpoints;

public class Auth : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(Login, "login");
        groupBuilder.MapPost(Register, "register");
        groupBuilder.MapPost(RefreshToken, "refresh");
    }

    public static async Task<Ok<AuthResult>> Login(ISender sender, LoginCommand command)
    {
        var result = await sender.Send(command);
        return TypedResults.Ok(result);
    }

    public static async Task<Ok<AuthResult>> Register(ISender sender, RegisterCommand command)
    {
        var result = await sender.Send(command);
        return TypedResults.Ok(result);
    }

    public static async Task<Ok<TokenResult>> RefreshToken(ISender sender, RefreshTokenCommand command)
    {
        var result = await sender.Send(command);
        return TypedResults.Ok(result);
    }
}
