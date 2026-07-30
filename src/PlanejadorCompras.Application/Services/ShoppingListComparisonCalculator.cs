using PlanejadorCompras.Application.Common.Dtos.Models;
using PlanejadorCompras.Application.Common.Dtos.Responses;
using ItemQuoteEntity = PlanejadorCompras.Domain.Entities.ItemQuote;
using ShoppingItemEntity = PlanejadorCompras.Domain.Entities.ShoppingItem;

namespace PlanejadorCompras.Application.Services;

public sealed class ShoppingListComparisonCalculator
{
    public EqualizationResponseDto CalculateEqualization(
        Guid shoppingListId,
        IReadOnlyCollection<ShoppingItemEntity> items,
        IReadOnlyCollection<ItemQuoteEntity> quotes,
        IReadOnlyDictionary<Guid, string> supplierNames)
    {
        if (items.Count == 0)
        {
            return new EqualizationResponseDto(
                shoppingListId,
                Array.Empty<string>(),
                Array.Empty<EqualizationItemRowDto>());
        }

        if (quotes.Count == 0)
        {
            var emptyItemRows = items
                .Select(item => new EqualizationItemRowDto(
                    item.Id,
                    item.Name,
                    item.Quantity,
                    item.Unit,
                    Array.Empty<EqualizationQuoteDto>()))
                .ToList();

            return new EqualizationResponseDto(
                shoppingListId,
                Array.Empty<string>(),
                emptyItemRows);
        }

        var suppliers = quotes
            .Select(quote => supplierNames[quote.SupplierId])
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var itemRows = items
            .Select(item =>
            {
                var itemQuotes = quotes
                    .Where(quote => quote.ShoppingItemId == item.Id)
                    .GroupBy(quote => quote.SupplierId)
                    .Select(group => group.MinBy(quote => quote.UnitPrice)!)
                    .Select(quote => new EqualizationQuoteDto(
                        quote.SupplierId,
                        supplierNames[quote.SupplierId],
                        quote.UnitPrice,
                        quote.UnitPrice * item.Quantity))
                    .OrderBy(quote => quote.SupplierName, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                return new EqualizationItemRowDto(
                    item.Id,
                    item.Name,
                    item.Quantity,
                    item.Unit,
                    itemQuotes);
            })
            .ToList();

        return new EqualizationResponseDto(shoppingListId, suppliers, itemRows);
    }

    public BestSupplierBudgetResponseDto CalculateBestSupplierBudget(
        Guid shoppingListId,
        IReadOnlyCollection<ShoppingItemEntity> items,
        IReadOnlyCollection<ItemQuoteEntity> quotes,
        IReadOnlyDictionary<Guid, string> supplierNames)
    {
        if (items.Count == 0 || quotes.Count == 0)
        {
            return new BestSupplierBudgetResponseDto(
                shoppingListId,
                null,
                0m,
                Array.Empty<BestSupplierBudgetItemDto>());
        }

        var itemIds = items.Select(item => item.Id).ToHashSet();

        var bestSupplierData = quotes
            .Where(quote => itemIds.Contains(quote.ShoppingItemId))
            .GroupBy(quote => quote.SupplierId)
            .Select(supplierGroup =>
            {
                var lowestQuotes = supplierGroup
                    .GroupBy(quote => quote.ShoppingItemId)
                    .ToDictionary(
                        group => group.Key,
                        group => group.MinBy(quote => quote.UnitPrice)!);

                var supplierItems = items
                    .Where(item => lowestQuotes.ContainsKey(item.Id))
                    .Select(item =>
                    {
                        var quote = lowestQuotes[item.Id];
                        return new BestSupplierBudgetItemDto(
                            item.Id,
                            item.Name,
                            quote.UnitPrice,
                            item.Quantity,
                            quote.UnitPrice * item.Quantity);
                    })
                    .ToList();

                return new
                {
                    SupplierName = supplierNames[supplierGroup.Key],
                    Total = supplierItems.Sum(item => item.TotalItemPrice),
                    Items = supplierItems
                };
            })
            .Where(result => result.Items.Count == items.Count)
            .OrderBy(result => result.Total)
            .ThenBy(result => result.SupplierName, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault();

        return new BestSupplierBudgetResponseDto(
            shoppingListId,
            bestSupplierData?.SupplierName,
            bestSupplierData?.Total ?? 0m,
            bestSupplierData?.Items is { } bestSupplierItems
                ? bestSupplierItems
                : Array.Empty<BestSupplierBudgetItemDto>());
    }
}
