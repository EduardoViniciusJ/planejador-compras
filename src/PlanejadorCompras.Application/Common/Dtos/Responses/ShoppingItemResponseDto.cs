namespace PlanejadorCompras.Application.Common.Dtos.Responses;

public sealed record ShoppingItemResponseDto(
    Guid Id,
    Guid ShoppingListId,
    string Name,
    decimal Quantity,
    string Unit,
    DateTime CreatedAt);
