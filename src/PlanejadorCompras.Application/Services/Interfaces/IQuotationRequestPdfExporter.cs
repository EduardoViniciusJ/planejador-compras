using PlanejadorCompras.Application.Common.Dtos.Reports;

namespace PlanejadorCompras.Application.Services.Interfaces;

public interface IQuotationRequestPdfExporter
{
    Task<ExportedFileDto> ExportAsync(
        QuotationRequestReportDataDto reportData,
        CancellationToken cancellationToken = default);
}
