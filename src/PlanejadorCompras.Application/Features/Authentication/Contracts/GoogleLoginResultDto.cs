namespace PlanejadorCompras.Application.Features.Authentication.Contracts;

public sealed record GoogleLoginResultDto(
    string AccessToken,
    DateTime ExpiresAtUtc);
