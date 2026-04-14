using PlanejadorCompras.Application.Common.Dtos.Requests;
using PlanejadorCompras.Application.Common.Dtos.Responses;

namespace PlanejadorCompras.Application.Services.Interfaces;

public interface IShoppingListUseCase
{
    Task<ShoppingListResponseDto> CreateAsync(ShoppingListRequestDto request, Guid userId, CancellationToken cancellationToken = default);
    Task<ShoppingListResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<ShoppingListResponseDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<ShoppingListResponseDto?> UpdateAsync(Guid id, ShoppingListRequestDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
