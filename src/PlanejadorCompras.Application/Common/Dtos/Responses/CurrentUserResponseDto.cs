namespace PlanejadorCompras.Application.Common.Dtos.Responses;

public sealed record CurrentUserResponseDto(
    Guid Id,
    string Email,
    string Name);
