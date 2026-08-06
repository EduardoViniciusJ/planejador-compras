using PlanejadorCompras.Application.Features.Equalizations.Contracts;

namespace PlanejadorCompras.Application.UseCases.Interfaces;

public interface IGetShoppingListEqualizationUseCase
{
    Task<EqualizationResponseDto> ExecuteAsync(Guid shoppingListId, CancellationToken cancellationToken = default);
}
