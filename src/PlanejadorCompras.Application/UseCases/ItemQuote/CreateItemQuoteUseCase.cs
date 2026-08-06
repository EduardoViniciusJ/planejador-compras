using PlanejadorCompras.Application.Features.ItemQuotes.Contracts;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories.ItemQuote;
using PlanejadorCompras.Domain.Repositories.ShoppingItem;
using PlanejadorCompras.Domain.Repositories;
using PlanejadorCompras.Domain.Repositories.ShoppingListSupplier;
using ShoppingItemEntity = PlanejadorCompras.Domain.Entities.ShoppingItem;
using ItemQuoteEntity = PlanejadorCompras.Domain.Entities.ItemQuote;

namespace PlanejadorCompras.Application.UseCases.ItemQuote.Create;

public sealed class CreateItemQuoteUseCase
{
    private readonly IItemQuoteRepository _itemQuoteRepository;
    private readonly IShoppingItemRepository _shoppingItemRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IShoppingListAccessService _shoppingListAccessService;
    private readonly ISupplierAccessService _supplierAccessService;
    private readonly IShoppingListSupplierRepository _shoppingListSupplierRepository;

    public CreateItemQuoteUseCase(
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
        ItemQuoteRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var shoppingItem = await _shoppingItemRepository.GetByIdAsync(request.ShoppingItemId, cancellationToken);
        if (shoppingItem is null)
        {
            throw new NotFoundException("Shopping item not found.", "shopping_item_not_found");
        }

        await _shoppingListAccessService.GetForCurrentUserAsync(shoppingItem.ShoppingListId, cancellationToken);
        var supplier = await _supplierAccessService.GetForCurrentUserAsync(request.SupplierId, cancellationToken);
        if (!await _shoppingListSupplierRepository.ExistsAsync(
                shoppingItem.ShoppingListId,
                request.SupplierId,
                cancellationToken))
        {
            throw new NotFoundException(
                "Supplier is not assigned to this shopping list.",
                "shopping_list_supplier_not_found");
        }

        var itemQuote = ItemQuoteEntity.Create(request.ShoppingItemId, request.SupplierId, request.UnitPrice);
        await _itemQuoteRepository.AddAsync(itemQuote, cancellationToken);
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
