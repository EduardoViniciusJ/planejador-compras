using PlanejadorCompras.Application.Features.ItemQuotes.Contracts;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories.ItemQuote;
using PlanejadorCompras.Domain.Repositories.ShoppingItem;
using PlanejadorCompras.Domain.Repositories;
using PlanejadorCompras.Domain.Repositories.ShoppingListSupplier;

namespace PlanejadorCompras.Application.UseCases.ItemQuote;

public sealed class UpdateItemQuoteUseCase
{
    private readonly IItemQuoteRepository _itemQuoteRepository;
    private readonly IShoppingItemRepository _shoppingItemRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IShoppingListAccessService _shoppingListAccessService;
    private readonly ISupplierAccessService _supplierAccessService;
    private readonly IShoppingListSupplierRepository _shoppingListSupplierRepository;

    public UpdateItemQuoteUseCase(
        IItemQuoteRepository itemQuoteRepository,
        IShoppingItemRepository shoppingItemRepository,
        IUnitOfWork unitOfWork,
        IShoppingListAccessService shoppingListAccessService,
        ISupplierAccessService supplierAccessService,
        IShoppingListSupplierRepository shoppingListSupplierRepository)
    {
        _itemQuoteRepository = itemQuoteRepository;
        _shoppingItemRepository = shoppingItemRepository;
        _unitOfWork = unitOfWork;
        _shoppingListAccessService = shoppingListAccessService;
        _supplierAccessService = supplierAccessService;
        _shoppingListSupplierRepository = shoppingListSupplierRepository;
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
        var supplier = await _supplierAccessService.GetForCurrentUserAsync(request.SupplierId, cancellationToken);
        if (!await _shoppingListSupplierRepository.ExistsAsync(
                targetShoppingItem.ShoppingListId,
                request.SupplierId,
                cancellationToken))
        {
            throw new NotFoundException(
                "Supplier is not assigned to this shopping list.",
                "shopping_list_supplier_not_found");
        }

        itemQuote.Update(request.ShoppingItemId, request.SupplierId, request.UnitPrice);
        await _itemQuoteRepository.UpdateAsync(itemQuote, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return new ItemQuoteResponseDto(
            itemQuote.Id,
            itemQuote.ShoppingItemId,
            itemQuote.SupplierId,
            supplier.Name,
            itemQuote.UnitPrice,
            itemQuote.CreatedAt);
    }
}
