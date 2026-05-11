namespace PlanejadorCompras.Application.Common.Dtos.Requests;

public sealed record GenerateAccessTokenRequestDto(
    Guid UserId,
    string Email,
    string Name);
