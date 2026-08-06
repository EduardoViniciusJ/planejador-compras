using PlanejadorCompras.Application.Features.ShoppingLists.Contracts;

namespace PlanejadorCompras.Application.Services.Interfaces;

public interface IShoppingListOverviewQuery
{
    Task<IReadOnlyList<ShoppingListOverviewDto>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
