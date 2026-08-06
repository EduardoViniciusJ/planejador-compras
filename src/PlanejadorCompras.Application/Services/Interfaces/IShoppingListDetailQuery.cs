using PlanejadorCompras.Application.Features.ShoppingLists.Contracts;

namespace PlanejadorCompras.Application.Services.Interfaces;

public interface IShoppingListDetailQuery
{
    Task<ShoppingListDetailResponseDto?> GetByIdAsync(
        Guid userId,
        Guid shoppingListId,
        CancellationToken cancellationToken = default);
}
