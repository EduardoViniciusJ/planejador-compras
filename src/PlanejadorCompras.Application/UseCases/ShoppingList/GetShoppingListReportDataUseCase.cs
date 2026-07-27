using PlanejadorCompras.Application.Common.Dtos.Reports;
using PlanejadorCompras.Application.Services;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Application.UseCases.Interfaces;
using PlanejadorCompras.Domain.Repositories.ItemQuote;
using PlanejadorCompras.Domain.Repositories.ShoppingItem;
using PlanejadorCompras.Domain.Repositories.ShoppingListSupplier;

namespace PlanejadorCompras.Application.UseCases.ShoppingList;

public sealed class GetShoppingListReportDataUseCase : IGetShoppingListReportDataUseCase
{
    private readonly IShoppingListAccessService _shoppingListAccessService;
    private readonly IShoppingItemRepository _shoppingItemRepository;
    private readonly IItemQuoteRepository _itemQuoteRepository;
    private readonly IShoppingListSupplierRepository _shoppingListSupplierRepository;
    private readonly ShoppingListComparisonCalculator _comparisonCalculator;
    private readonly TimeProvider _timeProvider;

    public GetShoppingListReportDataUseCase(
        IShoppingListAccessService shoppingListAccessService,
        IShoppingItemRepository shoppingItemRepository,
        IItemQuoteRepository itemQuoteRepository,
        IShoppingListSupplierRepository shoppingListSupplierRepository,
        ShoppingListComparisonCalculator comparisonCalculator,
        TimeProvider timeProvider)
    {
        _shoppingListAccessService = shoppingListAccessService;
        _shoppingItemRepository = shoppingItemRepository;
        _itemQuoteRepository = itemQuoteRepository;
        _shoppingListSupplierRepository = shoppingListSupplierRepository;
        _comparisonCalculator = comparisonCalculator;
        _timeProvider = timeProvider;
    }

    public async Task<ShoppingListReportDataDto> ExecuteAsync(
        Guid shoppingListId,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(shoppingListId, Guid.Empty);

        var shoppingList = await _shoppingListAccessService.GetForCurrentUserAsync(
            shoppingListId,
            cancellationToken);
        var items = await _shoppingItemRepository.GetByShoppingListIdAsync(
            shoppingListId,
            cancellationToken);
        var quotes = await _itemQuoteRepository.GetByShoppingListIdAsync(
            shoppingListId,
            cancellationToken);
        var suppliers = await _shoppingListSupplierRepository.GetSuppliersAsync(
            shoppingListId,
            cancellationToken);

        var supplierNames = suppliers.ToDictionary(supplier => supplier.Id, supplier => supplier.Name);
        var supplierByName = suppliers.ToDictionary(
            supplier => supplier.Name,
            StringComparer.OrdinalIgnoreCase);

        var equalization = _comparisonCalculator.CalculateEqualization(
            shoppingListId,
            items,
            quotes,
            supplierNames);
        var bestSupplierBudget = _comparisonCalculator.CalculateBestSupplierBudget(
            shoppingListId,
            items,
            quotes,
            supplierNames);

        var reportItems = equalization.Items
            .Select(item => MapItem(item, supplierByName))
            .ToList();
        var reportSuppliers = MapSuppliers(suppliers, reportItems);
        var pendingItems = MapPendingItems(reportItems, reportSuppliers);
        var summary = BuildSummary(reportItems, reportSuppliers, bestSupplierBudget);

        return new ShoppingListReportDataDto(
            shoppingListId,
            shoppingList.Name,
            shoppingList.Description,
            shoppingList.CreatedAt,
            _timeProvider.GetUtcNow(),
            summary,
            reportSuppliers,
            reportItems,
            pendingItems);
    }

    private static ShoppingListReportItemDto MapItem(
        Common.Dtos.Models.EqualizationItemRowDto item,
        IReadOnlyDictionary<string, Domain.Entities.Supplier> supplierByName)
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
        IEnumerable<Domain.Entities.Supplier> suppliers,
        IReadOnlyCollection<ShoppingListReportItemDto> items)
    {
        return suppliers
            .OrderBy(supplier => supplier.Name, StringComparer.OrdinalIgnoreCase)
            .Select(supplier =>
            {
                var supplierQuotes = items
                    .SelectMany(item => item.Quotes)
                    .Where(quote => quote.SupplierId == supplier.Id)
                    .ToList();
                var quotedItemCount = supplierQuotes.Count;
                var missingItemCount = items.Count - quotedItemCount;

                return new ShoppingListReportSupplierDto(
                    supplier.Id,
                    supplier.Name,
                    quotedItemCount,
                    missingItemCount,
                    items.Count > 0 && missingItemCount == 0,
                    supplierQuotes.Sum(quote => quote.TotalPrice));
            })
            .ToList();
    }

    private static List<ShoppingListReportPendingItemDto> MapPendingItems(
        IReadOnlyCollection<ShoppingListReportItemDto> items,
        IReadOnlyCollection<ShoppingListReportSupplierDto> suppliers)
    {
        return items
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
    }

    private static ShoppingListReportSummaryDto BuildSummary(
        IReadOnlyCollection<ShoppingListReportItemDto> items,
        IReadOnlyCollection<ShoppingListReportSupplierDto> suppliers,
        Common.Dtos.Responses.BestSupplierBudgetResponseDto bestSupplierBudget)
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
