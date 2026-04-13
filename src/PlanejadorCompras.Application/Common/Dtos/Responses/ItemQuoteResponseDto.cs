namespace PlanejadorCompras.Application.Common.Dtos.Responses;

public sealed record ItemQuoteResponseDto(
    Guid Id,
    Guid ShoppingItemId,
    string SupplierName,
    decimal UnitPrice,
    DateTime CreatedAt);
