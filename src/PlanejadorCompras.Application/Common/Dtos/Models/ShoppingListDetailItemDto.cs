namespace PlanejadorCompras.Application.Common.Dtos.Models;

public sealed record ShoppingListDetailItemDto(
    Guid Id,
    string Name,
    decimal Quantity,
    string Unit,
    DateTime CreatedAt,
    int QuoteCount,
    decimal? BestUnitPrice);
