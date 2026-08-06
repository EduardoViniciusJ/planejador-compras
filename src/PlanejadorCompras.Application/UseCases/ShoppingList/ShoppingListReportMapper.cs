using PlanejadorCompras.Application.Features.Equalizations.Contracts;
using PlanejadorCompras.Application.Features.Reports.Contracts;
using PlanejadorCompras.Application.Features.ShoppingLists.Contracts;
using SupplierEntity = PlanejadorCompras.Domain.Entities.Supplier;

namespace PlanejadorCompras.Application.UseCases.ShoppingList;

internal static class ShoppingListReportMapper
{
    internal static ShoppingListReportDataDto Map(
        Guid shoppingListId,
        Domain.Entities.ShoppingList shoppingList,
        IReadOnlyCollection<SupplierEntity> suppliers,
        EqualizationResponseDto equalization,
        BestSupplierBudgetResponseDto bestSupplierBudget,
        DateTimeOffset generatedAt)
    {
        var supplierByName = suppliers.ToDictionary(
            supplier => supplier.Name,
            StringComparer.OrdinalIgnoreCase);
        var reportItems = equalization.Items
            .Select(item => MapItem(item, supplierByName))
            .ToList();
        var reportSuppliers = MapSuppliers(suppliers, reportItems);

        return new ShoppingListReportDataDto(
            shoppingListId,
            shoppingList.Name,
            shoppingList.Description,
            shoppingList.CreatedAt,
            generatedAt,
            BuildSummary(reportItems, reportSuppliers, bestSupplierBudget),
            reportSuppliers,
            reportItems,
            MapPendingItems(reportItems, reportSuppliers));
    }

    private static ShoppingListReportItemDto MapItem(
        EqualizationItemRowDto item,
        IReadOnlyDictionary<string, SupplierEntity> supplierByName)
    {
        var lowestUnitPrice = item.Quotes.Any()
            ? item.Quotes.Min(quote => quote.UnitPrice)
            : (decimal?)null;
        var reportQuotes = item.Quotes
            .Select(quote =>
            {
                var supplier = supplierByName[quote.SupplierName];
                return new ShoppingListReportQuoteDto(
                    supplier.Id,
                    supplier.Name,
                    quote.UnitPrice,
                    quote.TotalPrice,
                    quote.UnitPrice == lowestUnitPrice);
            })
            .OrderBy(quote => quote.SupplierName, StringComparer.OrdinalIgnoreCase)
            .ToList();

        return new ShoppingListReportItemDto(
            item.ShoppingItemId,
            item.ItemName,
            item.Quantity,
            item.Unit,
            lowestUnitPrice,
            lowestUnitPrice * item.Quantity,
            reportQuotes);
    }

    private static List<ShoppingListReportSupplierDto> MapSuppliers(
        IEnumerable<SupplierEntity> suppliers,
        IReadOnlyCollection<ShoppingListReportItemDto> items) =>
        suppliers
            .OrderBy(supplier => supplier.Name, StringComparer.OrdinalIgnoreCase)
            .Select(supplier =>
            {
                var supplierQuotes = items
                    .SelectMany(item => item.Quotes)
                    .Where(quote => quote.SupplierId == supplier.Id)
                    .ToList();
                var missingItemCount = items.Count - supplierQuotes.Count;

                return new ShoppingListReportSupplierDto(
                    supplier.Id,
                    supplier.Name,
                    supplierQuotes.Count,
                    missingItemCount,
                    items.Count > 0 && missingItemCount == 0,
                    supplierQuotes.Sum(quote => quote.TotalPrice));
            })
            .ToList();

    private static List<ShoppingListReportPendingItemDto> MapPendingItems(
        IReadOnlyCollection<ShoppingListReportItemDto> items,
        IReadOnlyCollection<ShoppingListReportSupplierDto> suppliers) =>
        items
            .Where(item => suppliers.Count == 0 || item.Quotes.Count < suppliers.Count)
            .Select(item =>
            {
                var quotedSupplierIds = item.Quotes
                    .Select(quote => quote.SupplierId)
                    .ToHashSet();
                var missingSuppliers = suppliers
                    .Where(supplier => !quotedSupplierIds.Contains(supplier.SupplierId))
                    .ToList();

                return new ShoppingListReportPendingItemDto(
                    item.ShoppingItemId,
                    item.Name,
                    missingSuppliers.Select(supplier => supplier.SupplierId).ToList(),
                    missingSuppliers.Select(supplier => supplier.Name).ToList());
            })
            .ToList();

    private static ShoppingListReportSummaryDto BuildSummary(
        IReadOnlyCollection<ShoppingListReportItemDto> items,
        IReadOnlyCollection<ShoppingListReportSupplierDto> suppliers,
        BestSupplierBudgetResponseDto bestSupplierBudget)
    {
        var quotedItems = items.Count(item => item.Quotes.Count > 0);
        var quotedPriceCount = items.Sum(item => item.Quotes.Count);
        var expectedPriceCount = items.Count * suppliers.Count;
        var coveragePercentage = expectedPriceCount == 0
            ? 0m
            : quotedPriceCount * 100m / expectedPriceCount;
        var hasCompleteBestChoice = items.Count > 0 && quotedItems == items.Count;
        var bestChoiceTotal = items.Sum(item => item.LowestTotalPrice ?? 0m);
        var bestCompleteSupplier = bestSupplierBudget.BestSupplierName is null
            ? null
            : suppliers.FirstOrDefault(
                supplier => string.Equals(
                    supplier.Name,
                    bestSupplierBudget.BestSupplierName,
                    StringComparison.OrdinalIgnoreCase));
        decimal? bestCompleteSupplierTotal = bestCompleteSupplier is null
            ? null
            : bestSupplierBudget.TotalPrice;
        decimal? potentialSavings = hasCompleteBestChoice && bestCompleteSupplierTotal.HasValue
            ? bestCompleteSupplierTotal.Value - bestChoiceTotal
            : null;

        return new ShoppingListReportSummaryDto(
            items.Count,
            suppliers.Count,
            quotedItems,
            quotedPriceCount,
            expectedPriceCount,
            coveragePercentage,
            bestChoiceTotal,
            hasCompleteBestChoice,
            bestCompleteSupplier?.SupplierId,
            bestCompleteSupplier?.Name,
            bestCompleteSupplierTotal,
            potentialSavings);
    }
}
