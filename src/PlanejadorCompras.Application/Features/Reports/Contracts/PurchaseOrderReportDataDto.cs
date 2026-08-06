namespace PlanejadorCompras.Application.Features.Reports.Contracts;

public sealed record PurchaseOrderReportDataDto(
    string Code,
    string ShoppingListName,
    string SupplierName,
    string BuyerName,
    string? BuyerEmail,
    DateOnly? ExpectedDeliveryDate,
    string? DeliveryAddress,
    string? PaymentTerms,
    string? Notes,
    string Status,
    DateTime CreatedAtUtc,
    decimal TotalPrice,
    IReadOnlyCollection<PurchaseOrderReportItemDto> Items);
