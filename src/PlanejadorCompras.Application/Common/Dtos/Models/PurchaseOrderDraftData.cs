using PlanejadorCompras.Domain.Entities;

namespace PlanejadorCompras.Application.Common.Dtos.Models;

public sealed record PurchaseOrderDraftData(
    Guid ShoppingListId,
    string ShoppingListName,
    Guid SupplierId,
    string SupplierName,
    int TotalShoppingListItemCount,
    IReadOnlyCollection<PurchaseOrderItemSnapshot> Items,
    Guid? EqualizationId = null)
{
    public bool HasCompleteCoverage => Items.Count == TotalShoppingListItemCount;

    public decimal TotalPrice => Items.Sum(item => item.Quantity * item.UnitPrice);
}
