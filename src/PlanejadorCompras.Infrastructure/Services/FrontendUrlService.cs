using Microsoft.Extensions.Configuration;
using PlanejadorCompras.Application.Services.Interfaces;

namespace PlanejadorCompras.Infrastructure.Services;

public sealed class FrontendUrlService : IFrontendUrlService
{
    private readonly string _baseUrl;

    public FrontendUrlService(IConfiguration configuration)
    {
        var configuredBaseUrl = configuration["Frontend:BaseUrl"];

        if (!Uri.TryCreate(configuredBaseUrl, UriKind.Absolute, out var baseUri)
            || (baseUri.Scheme != Uri.UriSchemeHttp && baseUri.Scheme != Uri.UriSchemeHttps))
        {
            throw new InvalidOperationException(
                "Missing or invalid configuration 'Frontend:BaseUrl'.");
        }

        _baseUrl = baseUri.GetLeftPart(UriPartial.Authority).TrimEnd('/');
    }

    public string BuildEmailConfirmationUrl(string token) =>
        BuildUrl("confirmar-email", token);

    public string BuildPasswordResetUrl(string token) =>
        BuildUrl("reset-password", token);

    private string BuildUrl(string path, string token) =>
        $"{_baseUrl}/{path}?token={Uri.EscapeDataString(token)}";
}
