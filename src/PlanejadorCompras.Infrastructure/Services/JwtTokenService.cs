using PlanejadorCompras.Application.Features.Authentication.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PlanejadorCompras.Application.Services.Interfaces;

namespace PlanejadorCompras.Infrastructure.Services;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly string _issuer;
    private readonly string _audience;
    private readonly byte[] _secretKey;
    private readonly int _expiresInMinutes;

    public JwtTokenService(IConfiguration configuration)
    {
        _issuer = GetRequiredConfigurationValue(configuration, "Authentication:Jwt:Issuer");

        _audience = GetRequiredConfigurationValue(configuration, "Authentication:Jwt:Audience");

        var secretKey = GetRequiredConfigurationValue(configuration, "Authentication:Jwt:SecretKey");

        _secretKey = Encoding.UTF8.GetBytes(secretKey);
        _expiresInMinutes = int.TryParse(configuration["Authentication:Jwt:ExpiresInMinutes"], out var minutes) && minutes > 0
            ? minutes
            : 60;
    }

    public GenerateAccessTokenResponseDto GenerateAccessToken(GenerateAccessTokenRequestDto request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Email);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Name);
        ArgumentOutOfRangeException.ThrowIfEqual(request.UserId, Guid.Empty);

        var now = DateTime.UtcNow;
        var expiresAt = now.AddMinutes(_expiresInMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, request.UserId.ToString()),
            new(JwtRegisteredClaimNames.Email, request.Email.Trim().ToLowerInvariant()),
            new(JwtRegisteredClaimNames.Name, request.Name.Trim()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var signingKey = new SymmetricSecurityKey(_secretKey);
        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            notBefore: now,
            expires: expiresAt,
            signingCredentials: signingCredentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return new GenerateAccessTokenResponseDto(accessToken, expiresAt);
    }

    private static string GetRequiredConfigurationValue(IConfiguration configuration, string key)
    {
        var value = configuration[key];

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Missing configuration '{key}'.");
        }

        return value;
    }
}
