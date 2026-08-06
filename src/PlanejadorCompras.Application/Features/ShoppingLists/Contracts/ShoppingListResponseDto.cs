namespace PlanejadorCompras.Application.Features.ShoppingLists.Contracts;

public sealed record ShoppingListResponseDto(
    Guid Id,
    string Name,
    string? Description,
    DateTime CreatedAt);
