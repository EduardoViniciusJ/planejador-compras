using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories.ItemQuote;
using PlanejadorCompras.Domain.Repositories.ShoppingItem;

namespace PlanejadorCompras.Application.UseCases.ItemQuote;

public sealed class GetItemQuotesByShoppingItemIdUseCase
{
    private readonly IItemQuoteRepository _itemQuoteRepository;
    private readonly IShoppingItemRepository _shoppingItemRepository;
    private readonly IShoppingListAccessService _shoppingListAccessService;

    public GetItemQuotesByShoppingItemIdUseCase(
        IItemQuoteRepository itemQuoteRepository,
        IShoppingItemRepository shoppingItemRepository,
        IShoppingListAccessService shoppingListAccessService)
    {
        _itemQuoteRepository = itemQuoteRepository;
        _shoppingItemRepository = shoppingItemRepository;
        _shoppingListAccessService = shoppingListAccessService;
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
        return itemQuotes.Select(iq => new ItemQuoteResponseDto(
            iq.Id,
            iq.ShoppingItemId,
            iq.SupplierName,
            iq.UnitPrice,
            iq.CreatedAt)).ToList();
    }
}
