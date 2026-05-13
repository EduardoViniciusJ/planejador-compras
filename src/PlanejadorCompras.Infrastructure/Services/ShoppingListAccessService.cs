using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories.ShoppingList;
using ShoppingListEntity = PlanejadorCompras.Domain.Entities.ShoppingList;

namespace PlanejadorCompras.Infrastructure.Services;

public sealed class ShoppingListAccessService : IShoppingListAccessService
{
    private readonly IShoppingListRepository _shoppingListRepository;
    private readonly ICurrentUser _currentUser;

    public ShoppingListAccessService(
        IShoppingListRepository shoppingListRepository,
        ICurrentUser currentUser)
    {
        _shoppingListRepository = shoppingListRepository;
        _currentUser = currentUser;
    }

    public async Task<ShoppingListEntity> GetForCurrentUserAsync(
        Guid shoppingListId,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(shoppingListId, Guid.Empty);

        var shoppingList = await _shoppingListRepository.GetByIdAsync(shoppingListId, cancellationToken);
        if (shoppingList is null || shoppingList.UserId != _currentUser.UserId)
        {
            throw new NotFoundException("Shopping list not found.", "shopping_list_not_found");
        }

        return shoppingList;
    }
}
