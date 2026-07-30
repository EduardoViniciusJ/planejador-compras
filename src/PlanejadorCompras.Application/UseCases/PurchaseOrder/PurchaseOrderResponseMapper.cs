using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Domain.Entities;
using PurchaseOrderEntity = PlanejadorCompras.Domain.Entities.PurchaseOrder;

namespace PlanejadorCompras.Application.UseCases.PurchaseOrder;

internal static class PurchaseOrderResponseMapper
{
    public static PurchaseOrderSummaryResponseDto ToSummary(PurchaseOrderEntity order) =>
        new(
            order.Id,
            order.Code,
            order.ShoppingListName,
            order.SupplierName,
            order.BuyerName,
            order.Items.Count,
            order.TotalPrice,
            ToApiStatus(order.Status),
            order.CreatedAtUtc,
            order.ExpectedDeliveryDate);

    public static PurchaseOrderDetailResponseDto ToDetail(PurchaseOrderEntity order) =>
        new(
            order.Id,
            order.Code,
            order.SourceShoppingListId,
            order.ShoppingListName,
            order.SupplierId,
            order.SupplierName,
            order.BuyerName,
            order.BuyerEmail,
            order.ExpectedDeliveryDate,
            order.DeliveryAddress,
            order.PaymentTerms,
            order.Notes,
            ToApiStatus(order.Status),
            order.CreatedAtUtc,
            order.UpdatedAtUtc,
            order.CompletedAtUtc,
            order.CancelledAtUtc,
            order.TotalPrice,
            order.Items
                .Select(ToItem)
                .ToList(),
            order.SourceEqualizationId);

    public static PurchaseOrderItemResponseDto ToItem(PurchaseOrderItem item) =>
        new(
            item.SourceShoppingItemId,
            item.Name,
            item.Quantity,
            item.Unit,
            item.UnitPrice,
            item.TotalPrice);

    public static string ToApiStatus(PurchaseOrderStatus status) =>
        status switch
        {
            PurchaseOrderStatus.Issued => "issued",
            PurchaseOrderStatus.Completed => "completed",
            PurchaseOrderStatus.Cancelled => "cancelled",
            _ => throw new ArgumentOutOfRangeException(nameof(status))
        };

    public static string ToDisplayStatus(PurchaseOrderStatus status) =>
        status switch
        {
            PurchaseOrderStatus.Issued => "Emitido",
            PurchaseOrderStatus.Completed => "Concluído",
            PurchaseOrderStatus.Cancelled => "Cancelado",
            _ => throw new ArgumentOutOfRangeException(nameof(status))
        };
}
