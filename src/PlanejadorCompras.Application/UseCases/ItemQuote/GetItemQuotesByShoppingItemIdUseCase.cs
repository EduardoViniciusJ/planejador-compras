using PlanejadorCompras.Application.Features.ItemQuotes.Contracts;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories.ItemQuote;
using PlanejadorCompras.Domain.Repositories.ShoppingItem;
using PlanejadorCompras.Domain.Repositories.Supplier;

namespace PlanejadorCompras.Application.UseCases.ItemQuote;

public sealed class GetItemQuotesByShoppingItemIdUseCase
{
    private readonly IItemQuoteRepository _itemQuoteRepository;
    private readonly IShoppingItemRepository _shoppingItemRepository;
    private readonly IShoppingListAccessService _shoppingListAccessService;
    private readonly ISupplierRepository _supplierRepository;

    public GetItemQuotesByShoppingItemIdUseCase(
        IItemQuoteRepository itemQuoteRepository,
        IShoppingItemRepository shoppingItemRepository,
        IShoppingListAccessService shoppingListAccessService,
        ISupplierRepository supplierRepository)
    {
        _itemQuoteRepository = itemQuoteRepository;
        _shoppingItemRepository = shoppingItemRepository;
        _shoppingListAccessService = shoppingListAccessService;
        _supplierRepository = supplierRepository;
    }

    public async Task<List<ItemQuoteResponseDto>> ExecuteAsync(Guid shoppingItemId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(shoppingItemId, Guid.Empty);

        var shoppingItem = await _shoppingItemRepository.GetByIdAsync(shoppingItemId, cancellationToken);
        if (shoppingItem is null)
        {
            throw new NotFoundException("Shopping item not found.", "shopping_item_not_found");
        }

        await _shoppingListAccessService.GetForCurrentUserAsync(shoppingItem.ShoppingListId, cancellationToken);

        var itemQuotes = await _itemQuoteRepository.GetByShoppingItemIdAsync(shoppingItemId, cancellationToken);
        var suppliers = await _supplierRepository.GetByIdsAsync(
            itemQuotes.Select(quote => quote.SupplierId),
            cancellationToken);
        var supplierNames = suppliers.ToDictionary(supplier => supplier.Id, supplier => supplier.Name);

        return itemQuotes.Select(iq => new ItemQuoteResponseDto(
            iq.Id,
            iq.ShoppingItemId,
            iq.SupplierId,
            supplierNames[iq.SupplierId],
            iq.UnitPrice,
            iq.CreatedAt)).ToList();
    }
}
