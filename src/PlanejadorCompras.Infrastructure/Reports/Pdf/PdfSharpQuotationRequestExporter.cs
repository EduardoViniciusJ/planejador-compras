using MigraDoc.Rendering;
using PlanejadorCompras.Application.Common.Dtos.Reports;
using PlanejadorCompras.Application.Services.Interfaces;

namespace PlanejadorCompras.Infrastructure.Reports.Pdf;

public sealed class PdfSharpQuotationRequestExporter(
    QuotationRequestPdfDocumentBuilder documentBuilder)
    : IQuotationRequestPdfExporter
{
    private const string ContentType = "application/pdf";

    public Task<ExportedFileDto> ExportAsync(
        QuotationRequestReportDataDto reportData,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(reportData);
        cancellationToken.ThrowIfCancellationRequested();
        EmbeddedPdfFontResolver.EnsureRegistered();

        var document = documentBuilder.Build(reportData);
        var renderer = new PdfDocumentRenderer { Document = document };
        renderer.RenderDocument();
        cancellationToken.ThrowIfCancellationRequested();

        using var stream = new MemoryStream();
        renderer.PdfDocument.Save(stream, closeStream: false);
        var file = new ExportedFileDto(
            stream.ToArray(),
            ContentType,
            ReportFileNameBuilder.BuildQuotationRequestFileName(
                reportData.Code,
                "pdf"));

        renderer.PdfDocument.Dispose();
        return Task.FromResult(file);
    }
}
