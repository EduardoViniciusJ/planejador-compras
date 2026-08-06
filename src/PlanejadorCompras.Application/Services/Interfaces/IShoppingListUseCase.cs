using PlanejadorCompras.Application.Features.ShoppingLists.Contracts;

namespace PlanejadorCompras.Application.Services.Interfaces;

public interface IShoppingListUseCase
{
    Task<ShoppingListResponseDto> CreateAsync(ShoppingListRequestDto request, CancellationToken cancellationToken = default);
    Task<ShoppingListResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ShoppingListsOverviewResponseDto> GetByUserIdAsync(CancellationToken cancellationToken = default);
    Task<ShoppingListResponseDto> UpdateAsync(Guid id, ShoppingListRequestDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
