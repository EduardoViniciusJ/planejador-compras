using PlanejadorCompras.Application.Common.Dtos.Reports;

namespace PlanejadorCompras.Application.Services.Interfaces;

public interface IShoppingListPdfExporter
{
    Task<ExportedFileDto> ExportAsync(
        ShoppingListReportDataDto reportData,
        CancellationToken cancellationToken = default);
}
