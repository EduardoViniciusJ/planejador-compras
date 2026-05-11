using Microsoft.Extensions.Configuration;
using PlanejadorCompras.Application.Common.Dtos.Requests;
using PlanejadorCompras.Infrastructure.Services;

namespace PlanejadorCompras.Application.UnitTests.UseCases.Auth.JwtToken;

public sealed class JwtTokenServiceTestHelper
{
    public static readonly Guid DefaultUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public const string DefaultEmail = "user@test.com";
    public const string DefaultName = "Test User";
    public const string DefaultIssuer = "PlanejadorCompras";
    public const string DefaultAudience = "PlanejadorCompras";
    public const string DefaultSecretKey = "0123456789ABCDEF0123456789ABCDEF";
    public const int DefaultExpiresInMinutes = 60;

    public static IConfiguration CreateConfiguration(
        string? issuer = DefaultIssuer,
        string? audience = DefaultAudience,
        string? secretKey = DefaultSecretKey,
        int? expiresInMinutes = DefaultExpiresInMinutes)
    {
        var values = new Dictionary<string, string?>();

        if (issuer is not null)
        {
            values["Authentication:Jwt:Issuer"] = issuer;
        }

        if (audience is not null)
        {
            values["Authentication:Jwt:Audience"] = audience;
        }

        if (secretKey is not null)
        {
            values["Authentication:Jwt:SecretKey"] = secretKey;
        }

        if (expiresInMinutes is not null)
        {
            values["Authentication:Jwt:ExpiresInMinutes"] = expiresInMinutes.Value.ToString();
        }

        return new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
    }

    public static JwtTokenService CreateService(
        string? issuer = DefaultIssuer,
        string? audience = DefaultAudience,
        string? secretKey = DefaultSecretKey,
        int? expiresInMinutes = DefaultExpiresInMinutes)
    {
        return new JwtTokenService(CreateConfiguration(issuer, audience, secretKey, expiresInMinutes));
    }

    public static GenerateAccessTokenRequestDto CreateRequestDto(
        Guid? userId = null,
        string? email = DefaultEmail,
        string? name = DefaultName)
    {
        return new GenerateAccessTokenRequestDto(
            userId ?? DefaultUserId,
            email ?? DefaultEmail,
            name ?? DefaultName);
    }
}
