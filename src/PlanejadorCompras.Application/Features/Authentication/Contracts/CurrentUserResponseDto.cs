namespace PlanejadorCompras.Application.Features.Authentication.Contracts;

public sealed record CurrentUserResponseDto(
    Guid Id,
    string Email,
    string Name);
