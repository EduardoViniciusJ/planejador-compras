using PlanejadorCompras.Application.Features.Reports.Contracts;

namespace PlanejadorCompras.Application.UseCases.Interfaces;

public interface IExportShoppingListReportUseCase
{
    Task<ExportedFileDto> ExportPdfAsync(
        Guid shoppingListId,
        CancellationToken cancellationToken = default);

    Task<ExportedFileDto> ExportExcelAsync(
        Guid shoppingListId,
        CancellationToken cancellationToken = default);
}
