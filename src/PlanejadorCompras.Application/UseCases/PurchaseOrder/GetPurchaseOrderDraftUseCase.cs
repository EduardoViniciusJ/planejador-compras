using PlanejadorCompras.Application.Features.PurchaseOrders.Contracts;
using PlanejadorCompras.Application.Services;

namespace PlanejadorCompras.Application.UseCases.PurchaseOrder;

public sealed class GetPurchaseOrderDraftUseCase(
    PurchaseOrderDraftService draftService)
{
    public async Task<PurchaseOrderDraftResponseDto> ExecuteAsync(
        Guid shoppingListId,
        Guid supplierId,
        Guid? equalizationId = null,
        CancellationToken cancellationToken = default)
    {
        var draft = await draftService.BuildAsync(
            shoppingListId,
            supplierId,
            equalizationId,
            cancellationToken);

        return new PurchaseOrderDraftResponseDto(
            draft.ShoppingListId,
            draft.ShoppingListName,
            draft.SupplierId,
            draft.SupplierName,
            draft.TotalShoppingListItemCount,
            draft.Items.Count,
            draft.HasCompleteCoverage,
            draft.TotalPrice,
            draft.Items
                .Select(item => new PurchaseOrderItemResponseDto(
                    item.SourceShoppingItemId,
                    item.Name,
                    item.Quantity,
                    item.Unit,
                    item.UnitPrice,
                    item.Quantity * item.UnitPrice))
                .ToList(),
            draft.EqualizationId);
    }
}
