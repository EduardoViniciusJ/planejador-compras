using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Application.Services;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories.ItemQuote;
using PlanejadorCompras.Domain.Repositories.ShoppingItem;
using PlanejadorCompras.Application.UseCases.Interfaces;
using PlanejadorCompras.Domain.Repositories.Supplier;

namespace PlanejadorCompras.Application.UseCases.ShoppingList;

public sealed class CalculateBestSupplierBudgetUseCase : ICalculateBestSupplierBudgetUseCase
{
    private readonly IShoppingListAccessService _shoppingListAccessService;
    private readonly IShoppingItemRepository _shoppingItemRepository;
    private readonly IItemQuoteRepository _itemQuoteRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly ShoppingListComparisonCalculator _comparisonCalculator;

    public CalculateBestSupplierBudgetUseCase(
        IShoppingListAccessService shoppingListAccessService,
        IShoppingItemRepository shoppingItemRepository,
        IItemQuoteRepository itemQuoteRepository,
        ISupplierRepository supplierRepository,
        ShoppingListComparisonCalculator comparisonCalculator)
    {
        _shoppingListAccessService = shoppingListAccessService;
        _shoppingItemRepository = shoppingItemRepository;
        _itemQuoteRepository = itemQuoteRepository;
        _supplierRepository = supplierRepository;
        _comparisonCalculator = comparisonCalculator;
    }

    public async Task<BestSupplierBudgetResponseDto> ExecuteAsync(Guid shoppingListId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(shoppingListId, Guid.Empty);

        await _shoppingListAccessService.GetForCurrentUserAsync(shoppingListId, cancellationToken);

        var items = await _shoppingItemRepository.GetByShoppingListIdAsync(shoppingListId, cancellationToken);
        var quotes = await _itemQuoteRepository.GetByShoppingListIdAsync(shoppingListId, cancellationToken);
        var suppliers = await _supplierRepository.GetByIdsAsync(
            quotes.Select(quote => quote.SupplierId),
            cancellationToken);
        var supplierNames = suppliers.ToDictionary(supplier => supplier.Id, supplier => supplier.Name);

        return _comparisonCalculator.CalculateBestSupplierBudget(
            shoppingListId,
            items,
            quotes,
            supplierNames);
    }
}
