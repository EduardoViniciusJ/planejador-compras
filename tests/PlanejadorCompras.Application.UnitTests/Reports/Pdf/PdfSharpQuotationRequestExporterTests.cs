using PlanejadorCompras.Application.Features.Reports.Contracts;
using System.Text;
using PdfSharp.Pdf.IO;
using PlanejadorCompras.Infrastructure.Reports.Pdf;

namespace PlanejadorCompras.Application.UnitTests.Reports.Pdf;

public sealed class PdfSharpQuotationRequestExporterTests
{
    [Fact]
    public async Task ExportAsync_ShouldCreateReadableA4PortraitPdfWithoutBranding()
    {
        var exporter = new PdfSharpQuotationRequestExporter(
            new QuotationRequestPdfDocumentBuilder());
        var report = new QuotationRequestReportDataDto(
            Guid.Parse("10000000-0000-0000-0000-000000000001"),
            "SC-2026-ABC12345",
            "Compras para escritório",
            "Materiais para o escritório administrativo.",
            "Marina Lopes",
            "marina@example.com",
            new DateOnly(2026, 8, 4),
            new DateOnly(2026, 8, 8),
            "Rua das Compras, 100",
            "Enviar a proposta em reais.",
            new[]
            {
                new QuotationRequestReportItemDto("Mouse", 2, "un"),
                new QuotationRequestReportItemDto("Papel A4", 5, "cx")
            });

        var file = await exporter.ExportAsync(report);

        Assert.Equal("application/pdf", file.ContentType);
        Assert.Equal("solicitacao-cotacao-sc-2026-abc12345.pdf", file.FileName);
        Assert.Equal("%PDF-", Encoding.ASCII.GetString(file.Content, 0, 5));
        Assert.True(file.Content.Length > 2_000);

        using var stream = new MemoryStream(file.Content);
        using var document = PdfReader.Open(stream, PdfDocumentOpenMode.Import);

        Assert.Equal("Solicitação de cotação SC-2026-ABC12345", document.Info.Title);
        Assert.True(string.IsNullOrEmpty(document.Info.Author));
        Assert.All(
            document.Pages.Cast<PdfSharp.Pdf.PdfPage>(),
            page =>
            {
                Assert.True(page.Height.Point > page.Width.Point);
                Assert.InRange(page.Width.Millimeter, 209, 211);
                Assert.InRange(page.Height.Millimeter, 296, 298);
            });
    }
}
