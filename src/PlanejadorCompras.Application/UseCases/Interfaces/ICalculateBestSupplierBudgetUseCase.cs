using PlanejadorCompras.Application.Features.ShoppingLists.Contracts;

namespace PlanejadorCompras.Application.UseCases.Interfaces;

public interface ICalculateBestSupplierBudgetUseCase
{
    Task<BestSupplierBudgetResponseDto> ExecuteAsync(Guid shoppingListId, CancellationToken cancellationToken = default);
}
