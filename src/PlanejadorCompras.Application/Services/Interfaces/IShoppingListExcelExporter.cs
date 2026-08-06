using PlanejadorCompras.Application.Features.Reports.Contracts;

namespace PlanejadorCompras.Application.Services.Interfaces;

public interface IShoppingListExcelExporter
{
    Task<ExportedFileDto> ExportAsync(
        ShoppingListReportDataDto reportData,
        CancellationToken cancellationToken = default);
}
