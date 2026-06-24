namespace PlanejadorCompras.Application.Common.Dtos.Responses;

public sealed record ShoppingListResponseDto(
    Guid Id,
    string Name,
    string? Description,
    DateTime CreatedAt);
