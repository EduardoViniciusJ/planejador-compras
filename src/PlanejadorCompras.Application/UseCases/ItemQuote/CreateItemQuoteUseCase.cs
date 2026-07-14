using PlanejadorCompras.Application.Common.Dtos.Requests;
using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories.ItemQuote;
using PlanejadorCompras.Domain.Repositories.ShoppingItem;
using PlanejadorCompras.Domain.Repositories;
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

    public CreateItemQuoteUseCase(
        IItemQuoteRepository itemQuoteRepository,
        IShoppingItemRepository shoppingItemRepository,
        IUnitOfWork unitOfWork,
        IShoppingListAccessService shoppingListAccessService,
        ISupplierAccessService supplierAccessService)
    {
        _itemQuoteRepository = itemQuoteRepository;
        _shoppingItemRepository = shoppingItemRepository;
        _unitOfWork = unitOfWork;
        _shoppingListAccessService = shoppingListAccessService;
        _supplierAccessService = supplierAccessService;
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
