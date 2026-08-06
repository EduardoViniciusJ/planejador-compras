namespace PlanejadorCompras.Application.Features.PurchaseOrders.Contracts;

public sealed record PurchaseOrderItemResponseDto(
    Guid SourceShoppingItemId,
    string Name,
    decimal Quantity,
    string Unit,
    decimal UnitPrice,
    decimal TotalPrice);
