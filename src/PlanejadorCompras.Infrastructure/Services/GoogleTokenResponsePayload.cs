using System.Text.Json.Serialization;

namespace PlanejadorCompras.Infrastructure.Services;

internal sealed class GoogleTokenResponsePayload
{
    [JsonPropertyName("id_token")]
    public string? IdToken { get; init; }
}
