namespace PlanejadorCompras.Application.Common.Dtos.Responses;

public sealed record PurchaseOrderDetailResponseDto(
    Guid Id,
    string Code,
    Guid? ShoppingListId,
    string ShoppingListName,
    Guid? SupplierId,
    string SupplierName,
    string BuyerName,
    string? BuyerEmail,
    DateOnly? ExpectedDeliveryDate,
    string? DeliveryAddress,
    string? PaymentTerms,
    string? Notes,
    string Status,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc,
    DateTime? CompletedAtUtc,
    DateTime? CancelledAtUtc,
    decimal TotalPrice,
    IReadOnlyCollection<PurchaseOrderItemResponseDto> Items,
    Guid? EqualizationId = null);
