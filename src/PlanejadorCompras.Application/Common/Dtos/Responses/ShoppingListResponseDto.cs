namespace PlanejadorCompras.Application.Common.Dtos.Responses;

public sealed record ShoppingListResponseDto(
    Guid Id,
    Guid UserId,
    string Name,
    string? Description,
    DateTime CreatedAt);
