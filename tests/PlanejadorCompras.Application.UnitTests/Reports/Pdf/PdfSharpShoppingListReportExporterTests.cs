using System.Text;
using PdfSharp.Pdf.IO;
using PlanejadorCompras.Application.Common.Dtos.Reports;
using PlanejadorCompras.Infrastructure.Reports.Pdf;

namespace PlanejadorCompras.Application.UnitTests.Reports.Pdf;

public sealed class PdfSharpShoppingListReportExporterTests
{
    private static readonly Guid ShoppingListId =
        Guid.Parse("40000000-0000-0000-0000-000000000001");

    [Fact]
    public async Task ExportAsync_ShouldCreateNonEmptyLandscapePdf()
    {
        var exporter = CreateExporter();

        var exportedFile = await exporter.ExportAsync(
            CreateReportData(supplierCount: 2, itemCount: 3));

        Assert.Equal("application/pdf", exportedFile.ContentType);
        Assert.Equal(
            "compras-para-escritorio.pdf",
            exportedFile.FileName);
        Assert.True(exportedFile.Content.Length > 5_000);
        Assert.Equal(
            "%PDF-",
            Encoding.ASCII.GetString(exportedFile.Content, 0, 5));

        using var stream = new MemoryStream(exportedFile.Content);
        using var pdfDocument = PdfReader.Open(stream, PdfDocumentOpenMode.Import);

        Assert.NotEmpty(pdfDocument.Pages);
        Assert.All(
            pdfDocument.Pages.Cast<PdfSharp.Pdf.PdfPage>(),
            page => Assert.True(page.Width.Point > page.Height.Point));
        Assert.Equal(
            "Equalização - Compras para escritório",
            pdfDocument.Info.Title);
    }

    [Fact]
    public async Task ExportAsync_ShouldSplitLargeSupplierSetIntoPageGroups()
    {
        var exporter = CreateExporter();

        var exportedFile = await exporter.ExportAsync(
            CreateReportData(supplierCount: 9, itemCount: 2));

        using var stream = new MemoryStream(exportedFile.Content);
        using var pdfDocument = PdfReader.Open(stream, PdfDocumentOpenMode.Import);

        Assert.True(pdfDocument.PageCount >= 3);
        Assert.All(
            pdfDocument.Pages.Cast<PdfSharp.Pdf.PdfPage>(),
            page => Assert.True(page.Width.Point > page.Height.Point));
    }

    [Fact]
    public async Task ExportAsync_ShouldCreateReadablePdf_WhenReportIsEmpty()
    {
        var exporter = CreateExporter();
        var reportData = new ShoppingListReportDataDto(
            ShoppingListId,
            "Lista vazia",
            null,
            new DateTime(2026, 7, 1, 12, 0, 0, DateTimeKind.Utc),
            new DateTimeOffset(2026, 7, 25, 18, 30, 0, TimeSpan.Zero),
            new ShoppingListReportSummaryDto(
                0,
                0,
                0,
                0,
                0,
                0m,
                0m,
                false,
                null,
                null,
                null,
                null),
            Array.Empty<ShoppingListReportSupplierDto>(),
            Array.Empty<ShoppingListReportItemDto>(),
            Array.Empty<ShoppingListReportPendingItemDto>());

        var exportedFile = await exporter.ExportAsync(reportData);

        using var stream = new MemoryStream(exportedFile.Content);
        using var pdfDocument = PdfReader.Open(stream, PdfDocumentOpenMode.Import);

        Assert.NotEmpty(pdfDocument.Pages);
        Assert.True(exportedFile.Content.Length > 1_000);
    }

    [Fact]
    public async Task ExportAsync_ShouldHonorCancellation()
    {
        var exporter = CreateExporter();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => exporter.ExportAsync(
                CreateReportData(supplierCount: 2, itemCount: 2),
                cancellationTokenSource.Token));
    }

    private static PdfSharpShoppingListReportExporter CreateExporter()
    {
        return new PdfSharpShoppingListReportExporter(
            new ShoppingListPdfDocumentBuilder());
    }

    private static ShoppingListReportDataDto CreateReportData(
        int supplierCount,
        int itemCount)
    {
        var suppliers = Enumerable.Range(1, supplierCount)
            .Select(index => new
            {
                Id = CreateGuid(10_000 + index),
                Name = $"Fornecedor {index:00}"
            })
            .ToList();
        var items = new List<ShoppingListReportItemDto>();
        var pendingItems = new List<ShoppingListReportPendingItemDto>();

        for (var itemIndex = 1; itemIndex <= itemCount; itemIndex++)
        {
            var quantity = itemIndex;
            var itemId = CreateGuid(20_000 + itemIndex);
            var quotes = suppliers
                .Where((_, supplierIndex) =>
                    supplierIndex != suppliers.Count - 1 || itemIndex != itemCount)
                .Select((supplier, supplierIndex) =>
                {
                    var unitPrice = 10m + supplierIndex + itemIndex;

                    return new ShoppingListReportQuoteDto(
                        supplier.Id,
                        supplier.Name,
                        unitPrice,
                        unitPrice * quantity,
                        supplierIndex == 0);
                })
                .ToList();
            var lowestQuote = quotes.MinBy(quote => quote.UnitPrice);

            items.Add(
                new ShoppingListReportItemDto(
                    itemId,
                    $"Item de compra {itemIndex:00}",
                    quantity,
                    "un",
                    lowestQuote?.UnitPrice,
                    lowestQuote?.TotalPrice,
                    quotes));

            if (supplierCount > 0 && itemIndex == itemCount)
            {
                var missingSupplier = suppliers[^1];
                pendingItems.Add(
                    new ShoppingListReportPendingItemDto(
                        itemId,
                        $"Item de compra {itemIndex:00}",
                        new[] { missingSupplier.Id },
                        new[] { missingSupplier.Name }));
            }
        }

        var reportSuppliers = suppliers
            .Select((supplier, supplierIndex) =>
            {
                var supplierQuotes = items
                    .SelectMany(item => item.Quotes)
                    .Where(quote => quote.SupplierId == supplier.Id)
                    .ToList();
                var quotedItems = supplierQuotes.Count;
                var missingItems = itemCount - quotedItems;

                return new ShoppingListReportSupplierDto(
                    supplier.Id,
                    supplier.Name,
                    quotedItems,
                    missingItems,
                    itemCount > 0 && missingItems == 0,
                    supplierQuotes.Sum(quote => quote.TotalPrice));
            })
            .ToList();
        var quotedPriceCount = items.Sum(item => item.Quotes.Count);
        var expectedPriceCount = itemCount * supplierCount;
        var coverage = expectedPriceCount == 0
            ? 0m
            : quotedPriceCount * 100m / expectedPriceCount;
        var bestChoiceTotal = items.Sum(item => item.LowestTotalPrice ?? 0m);
        var bestCompleteSupplier = reportSuppliers.FirstOrDefault(
            supplier => supplier.HasCompleteCoverage);

        return new ShoppingListReportDataDto(
            ShoppingListId,
            "Compras para escritório",
            "Materiais necessários para a operação mensal do escritório.",
            new DateTime(2026, 7, 1, 12, 0, 0, DateTimeKind.Utc),
            new DateTimeOffset(2026, 7, 25, 18, 30, 0, TimeSpan.FromHours(-3)),
            new ShoppingListReportSummaryDto(
                itemCount,
                supplierCount,
                items.Count(item => item.Quotes.Count > 0),
                quotedPriceCount,
                expectedPriceCount,
                coverage,
                bestChoiceTotal,
                items.Count > 0 && items.All(item => item.Quotes.Count > 0),
                bestCompleteSupplier?.SupplierId,
                bestCompleteSupplier?.Name,
                bestCompleteSupplier?.QuotedTotal,
                bestCompleteSupplier is null
                    ? null
                    : bestCompleteSupplier.QuotedTotal - bestChoiceTotal),
            reportSuppliers,
            items,
            pendingItems);
    }

    private static Guid CreateGuid(int value)
    {
        var bytes = new byte[16];
        BitConverter.GetBytes(value).CopyTo(bytes, 0);
        return new Guid(bytes);
    }
}
