using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories.ShoppingItem;

namespace PlanejadorCompras.Application.UseCases.ShoppingItem;

public sealed class GetShoppingItemsByShoppingListIdUseCase
{
    private readonly IShoppingItemRepository _shoppingItemRepository;
    private readonly IShoppingListAccessService _shoppingListAccessService;

    public GetShoppingItemsByShoppingListIdUseCase(
        IShoppingItemRepository shoppingItemRepository,
        IShoppingListAccessService shoppingListAccessService)
    {
        _shoppingItemRepository = shoppingItemRepository;
        _shoppingListAccessService = shoppingListAccessService;
    }

    public async Task<List<ShoppingItemResponseDto>> ExecuteAsync(Guid shoppingListId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(shoppingListId, Guid.Empty);

        await _shoppingListAccessService.GetForCurrentUserAsync(shoppingListId, cancellationToken);

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
