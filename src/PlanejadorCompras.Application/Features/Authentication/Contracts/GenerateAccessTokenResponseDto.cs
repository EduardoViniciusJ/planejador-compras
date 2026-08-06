namespace PlanejadorCompras.Application.Features.Authentication.Contracts;

public sealed record GenerateAccessTokenResponseDto(
    string AccessToken,
    DateTime ExpiresAtUtc);
