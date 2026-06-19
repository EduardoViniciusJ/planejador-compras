using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories.ItemQuote;
using PlanejadorCompras.Domain.Repositories.ShoppingItem;
using PlanejadorCompras.Domain.Repositories;

namespace PlanejadorCompras.Application.UseCases.ItemQuote;

public sealed class DeleteItemQuoteUseCase
{
    private readonly IItemQuoteRepository _itemQuoteRepository;
    private readonly IShoppingItemRepository _shoppingItemRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IShoppingListAccessService _shoppingListAccessService;

    public DeleteItemQuoteUseCase(
        IItemQuoteRepository itemQuoteRepository,
        IShoppingItemRepository shoppingItemRepository,
        IUnitOfWork unitOfWork,
        IShoppingListAccessService shoppingListAccessService)
    {
        _itemQuoteRepository = itemQuoteRepository;
        _shoppingItemRepository = shoppingItemRepository;
        _unitOfWork = unitOfWork;
        _shoppingListAccessService = shoppingListAccessService;
    }

    public async Task ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
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

        var deleted = await _itemQuoteRepository.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            throw new NotFoundException("Item quote not found.", "item_quote_not_found");
        }

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
