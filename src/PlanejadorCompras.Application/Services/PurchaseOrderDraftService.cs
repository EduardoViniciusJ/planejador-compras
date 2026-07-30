using PlanejadorCompras.Application.Common.Dtos.Models;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Entities;
using PlanejadorCompras.Domain.Repositories.ItemQuote;
using PlanejadorCompras.Domain.Repositories.ShoppingItem;
using PlanejadorCompras.Domain.Repositories.ShoppingListSupplier;

namespace PlanejadorCompras.Application.Services;

public sealed class PurchaseOrderDraftService(
    IShoppingListAccessService shoppingListAccessService,
    ISupplierAccessService supplierAccessService,
    IShoppingListSupplierRepository shoppingListSupplierRepository,
    IShoppingItemRepository shoppingItemRepository,
    IItemQuoteRepository itemQuoteRepository,
    ISavedEqualizationAccessService savedEqualizationAccessService)
{
    public async Task<PurchaseOrderDraftData> BuildAsync(
        Guid shoppingListId,
        Guid supplierId,
        Guid? equalizationId = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(shoppingListId, Guid.Empty);
        ArgumentOutOfRangeException.ThrowIfEqual(supplierId, Guid.Empty);

        if (equalizationId.HasValue)
        {
            return await BuildFromSavedEqualizationAsync(
                shoppingListId,
                supplierId,
                equalizationId.Value,
                cancellationToken);
        }

        var shoppingList = await shoppingListAccessService.GetForCurrentUserAsync(
            shoppingListId,
            cancellationToken);
        var supplier = await supplierAccessService.GetForCurrentUserAsync(
            supplierId,
            cancellationToken);

        if (!await shoppingListSupplierRepository.ExistsAsync(
                shoppingListId,
                supplierId,
                cancellationToken))
        {
            throw new NotFoundException(
                "Supplier is not linked to this shopping list.",
                "shopping_list_supplier_not_found");
        }

        var items = await shoppingItemRepository.GetByShoppingListIdAsync(
            shoppingListId,
            cancellationToken);
        var quotes = await itemQuoteRepository.GetByShoppingListIdAsync(
            shoppingListId,
            cancellationToken);
        var lowestQuotesByItem = quotes
            .Where(quote => quote.SupplierId == supplierId)
            .GroupBy(quote => quote.ShoppingItemId)
            .ToDictionary(
                group => group.Key,
                group => group.MinBy(quote => quote.UnitPrice)!);

        var snapshots = items
            .Where(item => lowestQuotesByItem.ContainsKey(item.Id))
            .OrderBy(item => item.CreatedAt)
            .Select(item =>
            {
                var quote = lowestQuotesByItem[item.Id];
                return new PurchaseOrderItemSnapshot(
                    item.Id,
                    item.Name,
                    item.Quantity,
                    item.Unit,
                    quote.UnitPrice);
            })
            .ToList();

        if (snapshots.Count == 0)
        {
            throw new BadRequestException(
                "O fornecedor selecionado nao possui precos para esta lista.",
                "purchase_order_supplier_without_quotes");
        }

        return new PurchaseOrderDraftData(
            shoppingList.Id,
            shoppingList.Name,
            supplier.Id,
            supplier.Name,
            items.Count,
            snapshots);
    }

    private async Task<PurchaseOrderDraftData> BuildFromSavedEqualizationAsync(
        Guid shoppingListId,
        Guid supplierId,
        Guid equalizationId,
        CancellationToken cancellationToken)
    {
        var equalization = await savedEqualizationAccessService.GetForCurrentUserAsync(
            equalizationId,
            cancellationToken);

        if (equalization.SourceShoppingListId != shoppingListId)
        {
            throw new NotFoundException(
                "Equalization does not belong to this shopping list.",
                "equalization_shopping_list_not_found");
        }

        var quotedItems = equalization.Items
            .OrderBy(item => item.Position)
            .Select(item => new
            {
                Item = item,
                Quote = item.Quotes.SingleOrDefault(
                    quote => quote.SourceSupplierId == supplierId)
            })
            .Where(result => result.Quote is not null)
            .ToList();

        if (quotedItems.Count == 0)
        {
            throw new BadRequestException(
                "O fornecedor selecionado nao possui precos nesta equalizacao.",
                "purchase_order_supplier_without_quotes");
        }

        var supplierName = quotedItems[0].Quote!.SupplierName;
        var snapshots = quotedItems
            .Select(result => new PurchaseOrderItemSnapshot(
                result.Item.SourceShoppingItemId,
                result.Item.Name,
                result.Item.Quantity,
                result.Item.Unit,
                result.Quote!.UnitPrice))
            .ToList();

        return new PurchaseOrderDraftData(
            equalization.SourceShoppingListId,
            equalization.ShoppingListName,
            supplierId,
            supplierName,
            equalization.Items.Count,
            snapshots,
            equalization.Id);
    }
}
