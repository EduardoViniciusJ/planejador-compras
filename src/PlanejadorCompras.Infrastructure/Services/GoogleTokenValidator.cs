using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.Services.Interfaces;

namespace PlanejadorCompras.Infrastructure.Services;

public sealed class GoogleTokenValidator : IGoogleTokenValidator
{
    private readonly string _clientId;

    public GoogleTokenValidator(IConfiguration configuration)
    {
        var clientId = configuration["Authentication:Google:ClientId"];

        if (string.IsNullOrWhiteSpace(clientId))
        {
            throw new InvalidOperationException("Missing configuration 'Authentication:Google:ClientId'.");
        }

        _clientId = clientId;
    }

    public async Task<GoogleUserInfo> ValidateAsync(string idToken, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(idToken);

        GoogleJsonWebSignature.Payload payload;
        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(
                idToken,
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = [_clientId]
                });
        }
        catch (InvalidJwtException ex)
        {
            throw new UnauthorizedException("Invalid Google token.", "google_token_invalid", ex);
        }

        if (string.IsNullOrWhiteSpace(payload.Subject) || string.IsNullOrWhiteSpace(payload.Email))
        {
            throw new InvalidOperationException("Google token payload is missing required fields.");
        }

        if (!payload.EmailVerified)
        {
            throw new UnauthorizedException("Google account email is not verified.", "google_email_not_verified");
        }

        return new GoogleUserInfo(
            payload.Subject,
            payload.Email,
            payload.Name ?? string.Empty);
    }
}
