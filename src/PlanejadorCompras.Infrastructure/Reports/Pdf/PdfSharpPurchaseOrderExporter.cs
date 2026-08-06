using PlanejadorCompras.Application.Features.Reports.Contracts;
using MigraDoc.Rendering;
using PlanejadorCompras.Application.Services.Interfaces;

namespace PlanejadorCompras.Infrastructure.Reports.Pdf;

public sealed class PdfSharpPurchaseOrderExporter(
    PurchaseOrderPdfDocumentBuilder documentBuilder)
    : IPurchaseOrderPdfExporter
{
    private const string ContentType = "application/pdf";

    public Task<ExportedFileDto> ExportAsync(
        PurchaseOrderReportDataDto reportData,
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
            ReportFileNameBuilder.BuildPurchaseOrderFileName(
                reportData.Code,
                "pdf"));

        renderer.PdfDocument.Dispose();
        return Task.FromResult(file);
    }
}
