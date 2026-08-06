using PlanejadorCompras.Application.Features.Reports.Contracts;

namespace PlanejadorCompras.Application.Services.Interfaces;

public interface IQuotationRequestPdfExporter
{
    Task<ExportedFileDto> ExportAsync(
        QuotationRequestReportDataDto reportData,
        CancellationToken cancellationToken = default);
}
