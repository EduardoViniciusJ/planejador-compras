namespace PlanejadorCompras.Application.Common.Dtos.Responses;

public sealed record PurchaseOrderSummaryResponseDto(
    Guid Id,
    string Code,
    string ShoppingListName,
    string SupplierName,
    string BuyerName,
    int ItemCount,
    decimal TotalPrice,
    string Status,
    DateTime CreatedAtUtc,
    DateOnly? ExpectedDeliveryDate);
