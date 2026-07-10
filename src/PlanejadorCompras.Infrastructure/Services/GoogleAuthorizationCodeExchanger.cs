using System.Text.Json;
using Microsoft.Extensions.Configuration;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.Services.Interfaces;

namespace PlanejadorCompras.Infrastructure.Services;

public sealed class GoogleAuthorizationCodeExchanger : IGoogleAuthorizationCodeExchanger
{
    private const string GoogleTokenEndpoint = "https://oauth2.googleapis.com/token";

    private readonly HttpClient _httpClient;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _redirectUri;

    public GoogleAuthorizationCodeExchanger(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _clientId = GetRequiredConfigurationValue(configuration, "Authentication:Google:ClientId");
        _clientSecret = GetRequiredConfigurationValue(configuration, "Authentication:Google:ClientSecret");
        _redirectUri = GetRequiredConfigurationValue(configuration, "Authentication:Google:RedirectUri");
    }

    public async Task<string> ExchangeForIdTokenAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        using var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["code"] = code,
            ["client_id"] = _clientId,
            ["client_secret"] = _clientSecret,
            ["redirect_uri"] = _redirectUri,
            ["grant_type"] = "authorization_code"
        });

        using var response = await _httpClient.PostAsync(
            GoogleTokenEndpoint,
            content,
            cancellationToken);

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        var tokenResponse = DeserializeTokenResponse(responseBody);

        if (!response.IsSuccessStatusCode)
        {
            throw CreateInvalidGoogleLoginException();
        }

        if (string.IsNullOrWhiteSpace(tokenResponse?.IdToken))
        {
            throw CreateInvalidGoogleLoginException();
        }

        return tokenResponse.IdToken;
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

    private static GoogleTokenResponsePayload? DeserializeTokenResponse(string responseBody)
    {
        if (string.IsNullOrWhiteSpace(responseBody))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<GoogleTokenResponsePayload>(responseBody);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static UnauthorizedException CreateInvalidGoogleLoginException()
    {
        return new UnauthorizedException(
            "Unable to validate Google login.",
            "google_login_invalid");
    }
}
