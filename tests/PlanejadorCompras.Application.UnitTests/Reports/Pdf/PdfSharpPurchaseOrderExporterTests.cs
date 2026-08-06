using PlanejadorCompras.Application.Features.Reports.Contracts;
using System.Text;
using PdfSharp.Pdf.IO;
using PlanejadorCompras.Infrastructure.Reports.Pdf;

namespace PlanejadorCompras.Application.UnitTests.Reports.Pdf;

public sealed class PdfSharpPurchaseOrderExporterTests
{
    [Fact]
    public async Task ExportAsync_ShouldCreateReadableA4PortraitPdfWithoutBranding()
    {
        var exporter = new PdfSharpPurchaseOrderExporter(
            new PurchaseOrderPdfDocumentBuilder());
        var report = new PurchaseOrderReportDataDto(
            "PC-2026-ABC12345",
            "Compras para escritorio",
            "Fornecedor A",
            "Marina Lopes",
            "marina@example.com",
            new DateOnly(2026, 8, 15),
            "Rua das Compras, 100",
            "30 dias",
            "Entregar no almoxarifado.",
            "Emitido",
            new DateTime(2026, 7, 30, 15, 0, 0, DateTimeKind.Utc),
            2634.01m,
            new[]
            {
                new PurchaseOrderReportItemDto(
                    "Mouse",
                    1,
                    "un",
                    0.01m,
                    0.01m),
                new PurchaseOrderReportItemDto(
                    "Teclado",
                    3,
                    "un",
                    878m,
                    2634m)
            });

        var file = await exporter.ExportAsync(report);

        Assert.Equal("application/pdf", file.ContentType);
        Assert.Equal("pedido-compra-pc-2026-abc12345.pdf", file.FileName);
        Assert.Equal("%PDF-", Encoding.ASCII.GetString(file.Content, 0, 5));
        Assert.True(file.Content.Length > 2_000);

        using var stream = new MemoryStream(file.Content);
        using var document = PdfReader.Open(stream, PdfDocumentOpenMode.Import);

        Assert.Equal("Pedido de compra PC-2026-ABC12345", document.Info.Title);
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
