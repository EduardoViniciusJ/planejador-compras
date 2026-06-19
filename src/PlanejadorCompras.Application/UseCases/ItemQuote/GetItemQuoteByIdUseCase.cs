using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories.ItemQuote;
using PlanejadorCompras.Domain.Repositories.ShoppingItem;

namespace PlanejadorCompras.Application.UseCases.ItemQuote;

public sealed class GetItemQuoteByIdUseCase
{
    private readonly IItemQuoteRepository _itemQuoteRepository;
    private readonly IShoppingItemRepository _shoppingItemRepository;
    private readonly IShoppingListAccessService _shoppingListAccessService;

    public GetItemQuoteByIdUseCase(
        IItemQuoteRepository itemQuoteRepository,
        IShoppingItemRepository shoppingItemRepository,
        IShoppingListAccessService shoppingListAccessService)
    {
        _itemQuoteRepository = itemQuoteRepository;
        _shoppingItemRepository = shoppingItemRepository;
        _shoppingListAccessService = shoppingListAccessService;
    }

    public async Task<ItemQuoteResponseDto> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(id, Guid.Empty);

        var itemQuote = await _itemQuoteRepository.GetByIdAsync(id, cancellationToken);
        if (itemQuote is null)
        {
            throw new NotFoundException("Item quote not found.", "item_quote_not_found");
        }

        var shoppingItem = await _shoppingItemRepository.GetByIdAsync(itemQuote.ShoppingItemId, cancellationToken);
        if (shoppingItem is null)
        {
            throw new NotFoundException("Item quote not found.", "item_quote_not_found");
        }

        await _shoppingListAccessService.GetForCurrentUserAsync(shoppingItem.ShoppingListId, cancellationToken);

        return new ItemQuoteResponseDto(
            itemQuote.Id,
            itemQuote.ShoppingItemId,
            itemQuote.SupplierName,
            itemQuote.UnitPrice,
            itemQuote.CreatedAt);
    }
}
