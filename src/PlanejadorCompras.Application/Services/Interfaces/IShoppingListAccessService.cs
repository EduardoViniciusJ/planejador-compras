using ShoppingListEntity = PlanejadorCompras.Domain.Entities.ShoppingList;

namespace PlanejadorCompras.Application.Services.Interfaces;

public interface IShoppingListAccessService
{
    Task<ShoppingListEntity> GetForCurrentUserAsync(
        Guid shoppingListId,
        CancellationToken cancellationToken = default);
}
