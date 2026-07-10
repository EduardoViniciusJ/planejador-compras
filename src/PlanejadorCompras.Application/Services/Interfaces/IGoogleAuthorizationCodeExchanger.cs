namespace PlanejadorCompras.Application.Services.Interfaces;

public interface IGoogleAuthorizationCodeExchanger
{
    Task<string> ExchangeForIdTokenAsync(string code, CancellationToken cancellationToken = default);
}
