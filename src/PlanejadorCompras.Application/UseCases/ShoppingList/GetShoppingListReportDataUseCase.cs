using PlanejadorCompras.Application.Features.Reports.Contracts;
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

        var supplierNames = suppliers.ToDictionary(
            supplier => supplier.Id,
            supplier => supplier.Name);
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

        return ShoppingListReportMapper.Map(
            shoppingListId,
            shoppingList,
            suppliers,
            equalization,
            bestSupplierBudget,
            _timeProvider.GetUtcNow());
    }
}
