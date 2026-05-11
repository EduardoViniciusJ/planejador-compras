namespace PlanejadorCompras.Application.Common.Dtos.Responses;

public sealed record GenerateAccessTokenResponseDto(
    string AccessToken,
    DateTime ExpiresAtUtc);
