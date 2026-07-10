using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.Services.Interfaces;

namespace PlanejadorCompras.Infrastructure.Services;

public sealed class GoogleAuthorizationCodeExchanger : IGoogleAuthorizationCodeExchanger
{
    private const string GoogleTokenEndpoint = "https://oauth2.googleapis.com/token";

    private readonly HttpClient _httpClient;
    private readonly ILogger<GoogleAuthorizationCodeExchanger> _logger;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _redirectUri;

    public GoogleAuthorizationCodeExchanger(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<GoogleAuthorizationCodeExchanger> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
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
            _logger.LogWarning(
                "Google authorization code exchange failed. StatusCode: {StatusCode}; GoogleError: {GoogleError}; GoogleDescription: {GoogleDescription}; RedirectUri: {RedirectUri}; ClientId: {ClientId}",
                (int)response.StatusCode,
                tokenResponse?.Error,
                tokenResponse?.ErrorDescription,
                _redirectUri,
                _clientId);

            throw new UnauthorizedException(
                BuildGoogleUnauthorizedMessage(tokenResponse),
                BuildGoogleErrorCode(tokenResponse));
        }

        if (string.IsNullOrWhiteSpace(tokenResponse?.IdToken))
        {
            _logger.LogWarning(
                "Google authorization code exchange succeeded but did not return an id_token. RedirectUri: {RedirectUri}; ClientId: {ClientId}",
                _redirectUri,
                _clientId);

            throw new UnauthorizedException(
                "Google authorization response did not include an ID token.",
                "google_id_token_missing");
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

    private static GoogleTokenResponse? DeserializeTokenResponse(string responseBody)
    {
        if (string.IsNullOrWhiteSpace(responseBody))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<GoogleTokenResponse>(responseBody);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static string BuildGoogleUnauthorizedMessage(GoogleTokenResponse? tokenResponse)
    {
        if (string.IsNullOrWhiteSpace(tokenResponse?.Error))
        {
            return "Invalid Google authorization code.";
        }

        return $"Google rejected authorization code: {tokenResponse.Error}.";
    }

    private static string BuildGoogleErrorCode(GoogleTokenResponse? tokenResponse)
    {
        if (string.IsNullOrWhiteSpace(tokenResponse?.Error))
        {
            return "google_code_invalid";
        }

        var normalizedError = new string(tokenResponse.Error
            .ToLowerInvariant()
            .Select(character => char.IsLetterOrDigit(character) ? character : '_')
            .ToArray());

        return $"google_code_{normalizedError}";
    }

    private sealed record GoogleTokenResponse(
        [property: JsonPropertyName("id_token")]
        string? IdToken,
        [property: JsonPropertyName("error")]
        string? Error,
        [property: JsonPropertyName("error_description")]
        string? ErrorDescription);
    }
}
