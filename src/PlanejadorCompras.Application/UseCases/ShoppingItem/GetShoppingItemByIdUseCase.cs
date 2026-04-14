using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Domain.Repositories.ShoppingItem;

namespace PlanejadorCompras.Application.UseCases.ShoppingItem;

public sealed class GetShoppingItemByIdUseCase
{
    private readonly IShoppingItemRepository _shoppingItemRepository;

    public GetShoppingItemByIdUseCase(IShoppingItemRepository shoppingItemRepository)
    {
        _shoppingItemRepository = shoppingItemRepository;
    }

    public async Task<ShoppingItemResponseDto?> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(id, Guid.Empty);

        var shoppingItem = await _shoppingItemRepository.GetByIdAsync(id, cancellationToken);
        if (shoppingItem is null)
        {
            return null;
        }

        return new ShoppingItemResponseDto(
            shoppingItem.Id,
            shoppingItem.ShoppingListId,
            shoppingItem.Name,
            shoppingItem.Quantity,
            shoppingItem.Unit,
            shoppingItem.CreatedAt);
    }
}
