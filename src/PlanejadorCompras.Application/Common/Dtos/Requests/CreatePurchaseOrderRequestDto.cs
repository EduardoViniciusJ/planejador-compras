namespace PlanejadorCompras.Application.Common.Dtos.Requests;

public sealed record CreatePurchaseOrderRequestDto(
    Guid ShoppingListId,
    Guid SupplierId,
    string BuyerName,
    string? BuyerEmail,
    DateOnly? ExpectedDeliveryDate,
    string? DeliveryAddress,
    string? PaymentTerms,
    string? Notes,
    Guid? EqualizationId = null);
