namespace PlanejadorCompras.Application.Features.ItemQuotes.Contracts;

public sealed record ItemQuoteResponseDto(
    Guid Id,
    Guid ShoppingItemId,
    Guid SupplierId,
    string SupplierName,
    decimal UnitPrice,
    DateTime CreatedAt);
