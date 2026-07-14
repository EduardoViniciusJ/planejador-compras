using PlanejadorCompras.Application.Common.Dtos.Responses;

namespace PlanejadorCompras.Application.Services.Interfaces;

public interface IShoppingListDetailQuery
{
    Task<ShoppingListDetailResponseDto?> GetByIdAsync(
        Guid userId,
        Guid shoppingListId,
        CancellationToken cancellationToken = default);
}
