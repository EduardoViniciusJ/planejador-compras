using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Domain.Repositories.ShoppingItem;

namespace PlanejadorCompras.Application.UseCases.ShoppingItem;

public sealed class GetShoppingItemsByShoppingListIdUseCase
{
    private readonly IShoppingItemRepository _shoppingItemRepository;

    public GetShoppingItemsByShoppingListIdUseCase(IShoppingItemRepository shoppingItemRepository)
    {
        _shoppingItemRepository = shoppingItemRepository;
    }

    public async Task<List<ShoppingItemResponseDto>> ExecuteAsync(Guid shoppingListId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(shoppingListId, Guid.Empty);

        var shoppingItems = await _shoppingItemRepository.GetByShoppingListIdAsync(shoppingListId, cancellationToken);
        return shoppingItems.Select(si => new ShoppingItemResponseDto(
            si.Id,
            si.ShoppingListId,
            si.Name,
            si.Quantity,
            si.Unit,
            si.CreatedAt)).ToList();
    }
}
