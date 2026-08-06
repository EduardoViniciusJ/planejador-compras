using PlanejadorCompras.Application.Features.Reports.Contracts;

namespace PlanejadorCompras.Application.UseCases.Interfaces;

public interface IGetShoppingListReportDataUseCase
{
    Task<ShoppingListReportDataDto> ExecuteAsync(
        Guid shoppingListId,
        CancellationToken cancellationToken = default);
}
