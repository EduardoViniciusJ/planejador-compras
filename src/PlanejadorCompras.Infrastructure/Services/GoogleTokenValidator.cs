using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using PlanejadorCompras.Application.Services.Interfaces;

namespace PlanejadorCompras.Infrastructure.Services;

public sealed class GoogleTokenValidator : IGoogleTokenValidator
{
    private readonly string _clientId;

    public GoogleTokenValidator(IConfiguration configuration)
    {
        _clientId = configuration["Authentication:Google:ClientId"]
            ?? throw new InvalidOperationException("Missing configuration 'Authentication:Google:ClientId'.");
    }

    public async Task<GoogleUserInfo> ValidateAsync(string idToken, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(idToken);

        var payload = await GoogleJsonWebSignature.ValidateAsync(
            idToken,
            new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [_clientId]
            });

        if (string.IsNullOrWhiteSpace(payload.Subject) || string.IsNullOrWhiteSpace(payload.Email))
        {
            throw new InvalidOperationException("Google token payload is missing required fields.");
        }

        return new GoogleUserInfo(
            payload.Subject,
            payload.Email,
            payload.Name ?? string.Empty);
    }
}
