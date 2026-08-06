using PlanejadorCompras.Application.Features.Reports.Contracts;

namespace PlanejadorCompras.Application.Services.Interfaces;

public interface IShoppingListPdfExporter
{
    Task<ExportedFileDto> ExportAsync(
        ShoppingListReportDataDto reportData,
        CancellationToken cancellationToken = default);
}
