using PlanejadorCompras.Application.Features.ShoppingLists.Contracts;
using PlanejadorCompras.Application.Services.Interfaces;

namespace PlanejadorCompras.Application.UseCases.ShoppingList;

public sealed class GetShoppingListByIdUseCase
{
    private readonly IShoppingListAccessService _shoppingListAccessService;

    public GetShoppingListByIdUseCase(IShoppingListAccessService shoppingListAccessService)
    {
        _shoppingListAccessService = shoppingListAccessService;
    }

    public async Task<ShoppingListResponseDto> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var shoppingList = await _shoppingListAccessService.GetForCurrentUserAsync(id, cancellationToken);

        return new ShoppingListResponseDto(
            shoppingList.Id,
            shoppingList.Name,
            shoppingList.Description,
            shoppingList.CreatedAt);
    }
}
