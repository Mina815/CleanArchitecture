using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CleanArchitecture.Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CleanArchitecture.Infrastructure.Identity;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;

    public JwtService(IConfiguration configuration, UserManager<ApplicationUser> userManager)
    {
        _configuration = configuration;
        _userManager = userManager;
    }

    public async Task<(string accessToken, string refreshToken, DateTime expiresAt)> GenerateTokensAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        var expiresAt = DateTime.UtcNow.AddHours(2);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Role, role),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        if (user?.UserName != null)
            claims.Add(new(ClaimTypes.Name, user.UserName));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret is not configured.")));

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "Jamalek",
            audience: _configuration["Jwt:Audience"] ?? "JamalekApp",
            claims: claims,
            expires: expiresAt,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        user!.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = expiresAt.AddDays(7);
        await _userManager.UpdateAsync(user);

        return (accessToken, refreshToken, expiresAt);
    }

    public async Task<string?> ValidateRefreshTokenAsync(string userId, string refreshToken)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiry < DateTime.UtcNow)
            return null;

        return user.Id;
    }
}
