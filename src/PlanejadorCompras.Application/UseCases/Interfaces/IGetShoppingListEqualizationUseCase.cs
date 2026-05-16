using PlanejadorCompras.Application.Common.Dtos.Responses;

namespace PlanejadorCompras.Application.UseCases.Interfaces;

public interface IGetShoppingListEqualizationUseCase
{
    Task<EqualizationResponseDto> ExecuteAsync(Guid shoppingListId, CancellationToken cancellationToken = default);
}
