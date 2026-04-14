using PlanejadorCompras.Application.Common.Dtos.Requests;
using PlanejadorCompras.Application.Common.Dtos.Responses;

namespace PlanejadorCompras.Application.Services.Interfaces;

public interface IShoppingItemUseCase
{
    Task<ShoppingItemResponseDto> CreateAsync(ShoppingItemRequestDto request, CancellationToken cancellationToken = default);
    Task<ShoppingItemResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<ShoppingItemResponseDto>> GetByShoppingListIdAsync(Guid shoppingListId, CancellationToken cancellationToken = default);
    Task<ShoppingItemResponseDto?> UpdateAsync(Guid id, ShoppingItemRequestDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
