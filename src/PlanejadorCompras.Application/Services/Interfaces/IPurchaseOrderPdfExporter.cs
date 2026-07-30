using PlanejadorCompras.Application.Common.Dtos.Reports;

namespace PlanejadorCompras.Application.Services.Interfaces;

public interface IPurchaseOrderPdfExporter
{
    Task<ExportedFileDto> ExportAsync(
        PurchaseOrderReportDataDto reportData,
        CancellationToken cancellationToken = default);
}
