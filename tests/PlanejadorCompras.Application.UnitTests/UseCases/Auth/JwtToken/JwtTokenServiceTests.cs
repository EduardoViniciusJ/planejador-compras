using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using PlanejadorCompras.Infrastructure.Services;

namespace PlanejadorCompras.Application.UnitTests.UseCases.Auth.JwtToken;

public sealed class JwtTokenServiceTests
{
    [Fact]
    public void GenerateAccessToken_ShouldCreateToken_WithExpectedClaimsAndExpiration()
    {
        var service = JwtTokenServiceTestHelper.CreateService();
        var request = JwtTokenServiceTestHelper.CreateRequestDto(
            email: "  USER@Test.COM  ",
            name: "  Test User  ");
        var before = DateTime.UtcNow;

        var response = service.GenerateAccessToken(request);
        var token = new JwtSecurityTokenHandler().ReadJwtToken(response.AccessToken);
        var after = DateTime.UtcNow;

        Assert.Equal(JwtTokenServiceTestHelper.DefaultIssuer, token.Issuer);
        Assert.Contains(token.Audiences, audience => audience == JwtTokenServiceTestHelper.DefaultAudience);
        Assert.Contains(token.Claims, claim => claim.Type == JwtRegisteredClaimNames.Sub && claim.Value == request.UserId.ToString());
        Assert.Contains(token.Claims, claim => claim.Type == JwtRegisteredClaimNames.Email && claim.Value == request.Email.Trim().ToLowerInvariant());
        Assert.Contains(token.Claims, claim => claim.Type == JwtRegisteredClaimNames.Name && claim.Value == request.Name.Trim());
        Assert.Contains(token.Claims, claim => claim.Type == JwtRegisteredClaimNames.Jti && !string.IsNullOrWhiteSpace(claim.Value));
        Assert.Contains(token.Claims, claim => claim.Type == JwtRegisteredClaimNames.Iat && long.TryParse(claim.Value, out _));
        Assert.Equal(response.ExpiresAtUtc, token.ValidTo, TimeSpan.FromSeconds(1));
        Assert.InRange(response.ExpiresAtUtc, before.AddMinutes(59), after.AddMinutes(61));
    }

    [Fact]
    public void GenerateAccessToken_ShouldThrowInvalidOperationException_WhenIssuerIsMissing()
    {
        var action = () => JwtTokenServiceTestHelper.CreateService(issuer: null);

        Assert.Throws<InvalidOperationException>(action);
    }

    [Fact]
    public void GenerateAccessToken_ShouldThrowInvalidOperationException_WhenSecretKeyIsMissing()
    {
        var action = () => JwtTokenServiceTestHelper.CreateService(secretKey: null);

        Assert.Throws<InvalidOperationException>(action);
    }

    [Fact]
    public void GenerateAccessToken_ShouldThrowArgumentOutOfRangeException_WhenUserIdIsEmpty()
    {
        var service = JwtTokenServiceTestHelper.CreateService();
        var request = JwtTokenServiceTestHelper.CreateRequestDto(userId: Guid.Empty);

        Assert.Throws<ArgumentOutOfRangeException>(() => service.GenerateAccessToken(request));
    }

    [Fact]
    public void GenerateAccessToken_ShouldThrowArgumentException_WhenEmailIsEmpty()
    {
        var service = JwtTokenServiceTestHelper.CreateService();
        var request = JwtTokenServiceTestHelper.CreateRequestDto(email: string.Empty);

        Assert.Throws<ArgumentException>(() => service.GenerateAccessToken(request));
    }

    [Fact]
    public void GenerateAccessToken_ShouldThrowArgumentException_WhenNameIsEmpty()
    {
        var service = JwtTokenServiceTestHelper.CreateService();
        var request = JwtTokenServiceTestHelper.CreateRequestDto(name: string.Empty);

        Assert.Throws<ArgumentException>(() => service.GenerateAccessToken(request));
    }
}
