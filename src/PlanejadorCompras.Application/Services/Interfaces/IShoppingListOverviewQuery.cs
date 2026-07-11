using PlanejadorCompras.Application.Common.Dtos.Models;

namespace PlanejadorCompras.Application.Services.Interfaces;

public interface IShoppingListOverviewQuery
{
    Task<IReadOnlyList<ShoppingListOverviewDto>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
