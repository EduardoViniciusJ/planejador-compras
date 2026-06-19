using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories.ShoppingItem;

namespace PlanejadorCompras.Application.UseCases.ShoppingItem;

public sealed class GetShoppingItemByIdUseCase
{
    private readonly IShoppingItemRepository _shoppingItemRepository;
    private readonly IShoppingListAccessService _shoppingListAccessService;

    public GetShoppingItemByIdUseCase(
        IShoppingItemRepository shoppingItemRepository,
        IShoppingListAccessService shoppingListAccessService)
    {
        _shoppingItemRepository = shoppingItemRepository;
        _shoppingListAccessService = shoppingListAccessService;
    }

    public async Task<ShoppingItemResponseDto> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(id, Guid.Empty);

        var shoppingItem = await _shoppingItemRepository.GetByIdAsync(id, cancellationToken);
        if (shoppingItem is null)
        {
            throw new NotFoundException("Shopping item not found.", "shopping_item_not_found");
        }

        await _shoppingListAccessService.GetForCurrentUserAsync(shoppingItem.ShoppingListId, cancellationToken);

        return new ShoppingItemResponseDto(
            shoppingItem.Id,
            shoppingItem.ShoppingListId,
            shoppingItem.Name,
            shoppingItem.Quantity,
            shoppingItem.Unit,
            shoppingItem.CreatedAt);
    }
}
