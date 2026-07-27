using PlanejadorCompras.Application.Common.Dtos.Reports;

namespace PlanejadorCompras.Application.UseCases.Interfaces;

public interface IGetShoppingListReportDataUseCase
{
    Task<ShoppingListReportDataDto> ExecuteAsync(
        Guid shoppingListId,
        CancellationToken cancellationToken = default);
}
