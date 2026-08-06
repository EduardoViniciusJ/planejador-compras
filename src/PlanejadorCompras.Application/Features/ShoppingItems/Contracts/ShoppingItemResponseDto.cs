namespace PlanejadorCompras.Application.Features.ShoppingItems.Contracts;

public sealed record ShoppingItemResponseDto(
    Guid Id,
    Guid ShoppingListId,
    string Name,
    decimal Quantity,
    string Unit,
    DateTime CreatedAt);
