using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PlanejadorCompras.API.ErrorHandling;
using PlanejadorCompras.API.Security;

namespace PlanejadorCompras.API.Configuration;

internal static class ApiAuthenticationConfiguration
{
    internal static IServiceCollection AddApiAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var issuer = configuration.GetRequiredValue("Authentication:Jwt:Issuer");
        var audience = configuration.GetRequiredValue("Authentication:Jwt:Audience");
        var secretKey = configuration.GetRequiredSecret(
            "Authentication:Jwt:SecretKey",
            minimumByteLength: 32);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(secretKey))
                };

                options.Events = CreateJwtEvents();
            });

        services.AddAuthorization();
        return services;
    }

    private static JwtBearerEvents CreateJwtEvents() => new()
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies[
                AuthenticationConstants.AccessTokenCookieName];
            return Task.CompletedTask;
        },
        OnChallenge = async context =>
        {
            context.HandleResponse();

            if (!context.Response.HasStarted)
            {
                await ApiProblemDetailsWriter.WriteAsync(
                    context.HttpContext,
                    StatusCodes.Status401Unauthorized,
                    "Authentication is required.",
                    "authentication_required",
                    context.HttpContext.RequestAborted);
            }
        },
        OnForbidden = context => ApiProblemDetailsWriter.WriteAsync(
            context.HttpContext,
            StatusCodes.Status403Forbidden,
            "Access is forbidden.",
            "access_forbidden",
            context.HttpContext.RequestAborted)
    };

}
