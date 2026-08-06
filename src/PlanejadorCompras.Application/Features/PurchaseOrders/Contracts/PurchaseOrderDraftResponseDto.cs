namespace PlanejadorCompras.Application.Features.PurchaseOrders.Contracts;

public sealed record PurchaseOrderDraftResponseDto(
    Guid ShoppingListId,
    string ShoppingListName,
    Guid SupplierId,
    string SupplierName,
    int TotalShoppingListItemCount,
    int QuotedItemCount,
    bool HasCompleteCoverage,
    decimal TotalPrice,
    IReadOnlyCollection<PurchaseOrderItemResponseDto> Items,
    Guid? EqualizationId = null);
