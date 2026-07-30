namespace PlanejadorCompras.Application.Common.Dtos.Responses;

public sealed record PurchaseOrderItemResponseDto(
    Guid SourceShoppingItemId,
    string Name,
    decimal Quantity,
    string Unit,
    decimal UnitPrice,
    decimal TotalPrice);
