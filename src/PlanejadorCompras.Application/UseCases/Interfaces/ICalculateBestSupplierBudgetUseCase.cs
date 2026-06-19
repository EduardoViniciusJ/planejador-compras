using PlanejadorCompras.Application.Common.Dtos.Responses;

namespace PlanejadorCompras.Application.UseCases.Interfaces;

public interface ICalculateBestSupplierBudgetUseCase
{
    Task<BestSupplierBudgetResponseDto> ExecuteAsync(Guid shoppingListId, CancellationToken cancellationToken = default);
}
