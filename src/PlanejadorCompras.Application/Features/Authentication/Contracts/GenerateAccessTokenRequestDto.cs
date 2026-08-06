namespace PlanejadorCompras.Application.Features.Authentication.Contracts;

public sealed record GenerateAccessTokenRequestDto(
    Guid UserId,
    string Email,
    string Name);
