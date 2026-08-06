using PlanejadorCompras.Application.Features.Equalizations.Contracts;
using PlanejadorCompras.Application.Services;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories.ItemQuote;
using PlanejadorCompras.Domain.Repositories.ShoppingItem;
using PlanejadorCompras.Application.UseCases.Interfaces;
using PlanejadorCompras.Domain.Repositories.Supplier;

namespace PlanejadorCompras.Application.UseCases.ShoppingList;

public sealed class GetShoppingListEqualizationUseCase : IGetShoppingListEqualizationUseCase
{
    private readonly IShoppingListAccessService _shoppingListAccessService;
    private readonly IShoppingItemRepository _shoppingItemRepository;
    private readonly IItemQuoteRepository _itemQuoteRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly ShoppingListComparisonCalculator _comparisonCalculator;

    public GetShoppingListEqualizationUseCase(
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

    public async Task<EqualizationResponseDto> ExecuteAsync(Guid shoppingListId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(shoppingListId, Guid.Empty);

        await _shoppingListAccessService.GetForCurrentUserAsync(shoppingListId, cancellationToken);

        var items = await _shoppingItemRepository.GetByShoppingListIdAsync(shoppingListId, cancellationToken);
        var quotes = await _itemQuoteRepository.GetByShoppingListIdAsync(shoppingListId, cancellationToken);
        var supplierEntities = await _supplierRepository.GetByIdsAsync(
            quotes.Select(quote => quote.SupplierId),
            cancellationToken);
        var supplierNames = supplierEntities.ToDictionary(supplier => supplier.Id, supplier => supplier.Name);

        return _comparisonCalculator.CalculateEqualization(
            shoppingListId,
            items,
            quotes,
            supplierNames);
    }
}
