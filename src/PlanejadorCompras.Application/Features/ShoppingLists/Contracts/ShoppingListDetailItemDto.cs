namespace PlanejadorCompras.Application.Features.ShoppingLists.Contracts;

public sealed record ShoppingListDetailItemDto(
    Guid Id,
    string Name,
    decimal Quantity,
    string Unit,
    DateTime CreatedAt,
    int QuoteCount,
    decimal? BestUnitPrice);
