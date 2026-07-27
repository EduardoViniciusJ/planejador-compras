using PlanejadorCompras.Application.Common.Dtos.Reports;

namespace PlanejadorCompras.Application.Services.Interfaces;

public interface IShoppingListExcelExporter
{
    Task<ExportedFileDto> ExportAsync(
        ShoppingListReportDataDto reportData,
        CancellationToken cancellationToken = default);
}
