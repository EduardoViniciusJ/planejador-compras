namespace PlanejadorCompras.Application.Common.Dtos.Results;

public sealed record GoogleLoginResultDto(
    string AccessToken,
    DateTime ExpiresAtUtc);
