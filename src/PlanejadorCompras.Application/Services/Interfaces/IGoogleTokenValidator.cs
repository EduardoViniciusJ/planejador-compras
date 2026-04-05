namespace PlanejadorCompras.Application.Services.Interfaces;

public interface IGoogleTokenValidator
{
    Task<GoogleUserInfo> ValidateAsync(string idToken, CancellationToken cancellationToken = default);
}

public sealed record GoogleUserInfo(string GoogleId, string Email, string Name);
