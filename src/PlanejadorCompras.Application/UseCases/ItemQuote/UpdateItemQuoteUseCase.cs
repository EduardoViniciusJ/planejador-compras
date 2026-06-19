using PlanejadorCompras.Application.Common.Dtos.Requests;
using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories.ItemQuote;
using PlanejadorCompras.Domain.Repositories.ShoppingItem;
using PlanejadorCompras.Domain.Repositories;

namespace PlanejadorCompras.Application.UseCases.ItemQuote;

public sealed class UpdateItemQuoteUseCase
{
    private readonly IItemQuoteRepository _itemQuoteRepository;
    private readonly IShoppingItemRepository _shoppingItemRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IShoppingListAccessService _shoppingListAccessService;

    public UpdateItemQuoteUseCase(
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

    public async Task<ItemQuoteResponseDto> ExecuteAsync(
        Guid id,
        ItemQuoteRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentOutOfRangeException.ThrowIfEqual(id, Guid.Empty);

        var itemQuote = await _itemQuoteRepository.GetByIdAsync(id, cancellationToken);
        if (itemQuote is null)
        {
            throw new NotFoundException("Item quote not found.", "item_quote_not_found");
        }

        var currentShoppingItem = await _shoppingItemRepository.GetByIdAsync(itemQuote.ShoppingItemId, cancellationToken);
        if (currentShoppingItem is null)
        {
            throw new NotFoundException("Item quote not found.", "item_quote_not_found");
        }

        await _shoppingListAccessService.GetForCurrentUserAsync(currentShoppingItem.ShoppingListId, cancellationToken);

        var targetShoppingItem = await _shoppingItemRepository.GetByIdAsync(request.ShoppingItemId, cancellationToken);
        if (targetShoppingItem is null)
        {
            throw new NotFoundException("Shopping item not found.", "shopping_item_not_found");
        }

        await _shoppingListAccessService.GetForCurrentUserAsync(targetShoppingItem.ShoppingListId, cancellationToken);

        itemQuote.Update(request.ShoppingItemId, request.SupplierName, request.UnitPrice);
        await _itemQuoteRepository.UpdateAsync(itemQuote, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return new ItemQuoteResponseDto(
            itemQuote.Id,
            itemQuote.ShoppingItemId,
            itemQuote.SupplierName,
            itemQuote.UnitPrice,
            itemQuote.CreatedAt);
    }
}
