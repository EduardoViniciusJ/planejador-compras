using PlanejadorCompras.Application.Common.Dtos.Requests;
using PlanejadorCompras.Application.Common.Dtos.Responses;

namespace PlanejadorCompras.Application.Services.Interfaces;

public interface IItemQuoteUseCase
{
    Task<ItemQuoteResponseDto> CreateAsync(ItemQuoteRequestDto request, CancellationToken cancellationToken = default);
    Task<ItemQuoteResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<ItemQuoteResponseDto>> GetByShoppingItemIdAsync(Guid shoppingItemId, CancellationToken cancellationToken = default);
    Task<ItemQuoteResponseDto> UpdateAsync(Guid id, ItemQuoteRequestDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
