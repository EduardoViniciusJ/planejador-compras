using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using PlanejadorCompras.API.Controllers;
using PlanejadorCompras.Application.Features.Authentication.Contracts;
using PlanejadorCompras.Domain.Entities;
using PlanejadorCompras.Infrastructure.Services;

namespace PlanejadorCompras.Application.UnitTests.UseCases.Auth;

public sealed class AuthSecurityTests
{
    [Fact]
    public void RegisterRequest_ShouldRejectInvalidEmailAndShortPassword()
    {
        var request = new RegisterRequestDto("invalid", "short");

        var results = Validate(request);

        Assert.Equal(2, results.Count);
    }

    [Fact]
    public void ResetPasswordRequest_ShouldAcceptValidTokenAndPassword()
    {
        var request = new ResetPasswordRequestDto(new string('A', 64), "a secure password");

        Assert.Empty(Validate(request));
    }

    [Fact]
    public void UserToken_ShouldBeCryptographicallySizedAndUnique()
    {
        var first = UserToken.Create(Guid.NewGuid(), "PasswordReset", TimeSpan.FromHours(1));
        var second = UserToken.Create(Guid.NewGuid(), "PasswordReset", TimeSpan.FromHours(1));

        Assert.Equal(64, first.Token.Length);
        Assert.NotEqual(first.Token, second.Token);
    }

    [Theory]
    [InlineData(nameof(AuthController.Login), "auth-login")]
    [InlineData(nameof(AuthController.GoogleCodeLogin), "auth-login")]
    [InlineData(nameof(AuthController.Register), "auth-email")]
    [InlineData(nameof(AuthController.ForgotPassword), "auth-email")]
    [InlineData(nameof(AuthController.ResetPassword), "auth-email")]
    [InlineData(nameof(AuthController.ConfirmEmail), "auth-email")]
    public void PublicAuthEndpoint_ShouldApplyRateLimiting(string methodName, string policyName)
    {
        var method = typeof(AuthController).GetMethod(methodName);

        var attribute = Assert.Single(
            method!.GetCustomAttributes(typeof(EnableRateLimitingAttribute), true)
                .Cast<EnableRateLimitingAttribute>());
        Assert.Equal(policyName, attribute.PolicyName);
    }

    [Fact]
    public void FrontendUrlService_ShouldUseTrustedConfiguredOriginAndEncodeToken()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Frontend:BaseUrl"] = "https://app.example.com"
            })
            .Build();
        var service = new FrontendUrlService(configuration);

        var url = service.BuildPasswordResetUrl("token with spaces");

        Assert.Equal("https://app.example.com/reset-password?token=token%20with%20spaces", url);
    }

    private static List<ValidationResult> Validate(object value)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(value, new ValidationContext(value), results, true);
        return results;
    }
}
