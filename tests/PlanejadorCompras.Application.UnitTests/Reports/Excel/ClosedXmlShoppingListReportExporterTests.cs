using ClosedXML.Excel;
using PlanejadorCompras.Application.Common.Dtos.Reports;
using PlanejadorCompras.Infrastructure.Reports.Excel;

namespace PlanejadorCompras.Application.UnitTests.Reports.Excel;

public sealed class ClosedXmlShoppingListReportExporterTests
{
    private static readonly Guid ShoppingListId =
        Guid.Parse("10000000-0000-0000-0000-000000000001");
    private static readonly Guid SupplierAId =
        Guid.Parse("20000000-0000-0000-0000-000000000001");
    private static readonly Guid SupplierBId =
        Guid.Parse("20000000-0000-0000-0000-000000000002");
    private static readonly Guid PaperItemId =
        Guid.Parse("30000000-0000-0000-0000-000000000001");
    private static readonly Guid PenItemId =
        Guid.Parse("30000000-0000-0000-0000-000000000002");

    [Fact]
    public async Task ExportAsync_ShouldCreateTypedWorkbookWithExpectedWorksheets()
    {
        var exporter = new ClosedXmlShoppingListReportExporter();
        var reportData = CreateReportData();

        var exportedFile = await exporter.ExportAsync(reportData);

        Assert.Equal(
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            exportedFile.ContentType);
        Assert.Equal(
            "compras-para-escritorio.xlsx",
            exportedFile.FileName);
        Assert.NotEmpty(exportedFile.Content);
        Assert.Equal((byte)'P', exportedFile.Content[0]);
        Assert.Equal((byte)'K', exportedFile.Content[1]);

        using var stream = new MemoryStream(exportedFile.Content);
        using var workbook = new XLWorkbook(stream);

        Assert.Equal(
            new[] { "Resumo", "Mapa de preços", "Cotações", "Pendências" },
            workbook.Worksheets.Select(worksheet => worksheet.Name));

        var summary = workbook.Worksheet("Resumo");
        Assert.Equal("Compras para escritório", summary.Cell("B3").GetString());
        Assert.Equal(XLDataType.DateTime, summary.Cell("B5").DataType);
        Assert.Equal(XLDataType.Number, summary.Cell("B14").DataType);
        Assert.Equal(0.75m, summary.Cell("B14").GetValue<decimal>());
        Assert.Equal(21m, summary.Cell("B15").GetValue<decimal>());
        Assert.Equal(25m, summary.Cell("B18").GetValue<decimal>());
        Assert.Equal(4m, summary.Cell("B19").GetValue<decimal>());
        Assert.Contains("R$", summary.Cell("B15").Style.NumberFormat.Format);

        var priceMap = workbook.Worksheet("Mapa de preços");
        Assert.Equal("Fornecedor A - Preço unitário", priceMap.Cell("D1").GetString());
        Assert.Equal("Fornecedor B - Preço total", priceMap.Cell("G1").GetString());
        Assert.Equal(XLDataType.Number, priceMap.Cell("B2").DataType);
        Assert.Equal(2m, priceMap.Cell("B2").GetValue<decimal>());
        Assert.Equal(10m, priceMap.Cell("D2").GetValue<decimal>());
        Assert.Equal(8m, priceMap.Cell("F2").GetValue<decimal>());
        Assert.Equal(
            XLColor.FromHtml("#E2F0D9"),
            priceMap.Cell("F2").Style.Fill.BackgroundColor);
        Assert.Equal("Sem preço", priceMap.Cell("F3").GetString());
        Assert.Equal(
            XLColor.FromHtml("#FFF2CC"),
            priceMap.Cell("F3").Style.Fill.BackgroundColor);
        Assert.True(priceMap.AutoFilter.IsEnabled);

        var quotes = workbook.Worksheet("Cotações");
        Assert.Equal(3, quotes.RowsUsed().Count() - 1);
        Assert.Equal(XLDataType.Number, quotes.Cell("E2").DataType);
        Assert.Contains("R$", quotes.Cell("E2").Style.NumberFormat.Format);
        Assert.True(quotes.AutoFilter.IsEnabled);

        var pendingItems = workbook.Worksheet("Pendências");
        Assert.Equal("Caneta", pendingItems.Cell("A2").GetString());
        Assert.Equal("Fornecedor B", pendingItems.Cell("B2").GetString());
        Assert.Equal("Preço não informado", pendingItems.Cell("C2").GetString());
        Assert.True(pendingItems.AutoFilter.IsEnabled);
    }

    [Fact]
    public async Task ExportAsync_ShouldPersistUserValuesAsTextWithoutFormulas()
    {
        const string unsafeListName = "=HYPERLINK(\"https://example.com\";\"Lista\")";
        const string unsafeDescription = "+SUM(1;1)";
        const string unsafeItemName = "-1+2";
        const string unsafeSupplierName = "@SUM(A1:A2)";
        var exporter = new ClosedXmlShoppingListReportExporter();
        var reportData = CreateReportData(
            unsafeListName,
            unsafeDescription,
            unsafeItemName,
            unsafeSupplierName);

        var exportedFile = await exporter.ExportAsync(reportData);

        Assert.Equal(
            "hyperlink-https-example-com-lista.xlsx",
            exportedFile.FileName);

        using var stream = new MemoryStream(exportedFile.Content);
        using var workbook = new XLWorkbook(stream);

        AssertTextWithoutFormula(workbook.Worksheet("Resumo").Cell("B3"), unsafeListName);
        AssertTextWithoutFormula(
            workbook.Worksheet("Resumo").Cell("B4"),
            unsafeDescription);
        AssertTextWithoutFormula(
            workbook.Worksheet("Mapa de preços").Cell("A2"),
            unsafeItemName);
        AssertTextWithoutFormula(
            workbook.Worksheet("Mapa de preços").Cell("D1"),
            $"{unsafeSupplierName} - Preço unitário");
        AssertTextWithoutFormula(
            workbook.Worksheet("Cotações").Cell("D2"),
            unsafeSupplierName);
    }

    [Fact]
    public async Task ExportAsync_ShouldLimitFileNameAndFallbackToId_WhenNameHasNoSafeCharacters()
    {
        var exporter = new ClosedXmlShoppingListReportExporter();

        var limitedNameFile = await exporter.ExportAsync(
            CreateReportData(new string('A', 120)));
        var fallbackFile = await exporter.ExportAsync(
            CreateReportData(" /// \r\n "));

        Assert.Equal($"{new string('a', 80)}.xlsx", limitedNameFile.FileName);
        Assert.Equal("equalizacao.xlsx", fallbackFile.FileName);
    }

    [Fact]
    public async Task ExportAsync_ShouldHonorCancellation()
    {
        var exporter = new ClosedXmlShoppingListReportExporter();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => exporter.ExportAsync(
                CreateReportData(),
                cancellationTokenSource.Token));
    }

    [Fact]
    public async Task ExportAsync_ShouldCreateReadableWorkbook_WhenReportIsEmpty()
    {
        var exporter = new ClosedXmlShoppingListReportExporter();
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
        using var workbook = new XLWorkbook(stream);

        Assert.Equal(
            "Nenhum fornecedor cadastrado.",
            workbook.Worksheet("Resumo").Cell("A23").GetString());
        Assert.Equal(
            "Sem preços",
            workbook.Worksheet("Mapa de preços").Cell("D2").GetString());
        Assert.Equal(
            "Nenhuma cotação informada.",
            workbook.Worksheet("Cotações").Cell("A2").GetString());
        Assert.Equal(
            "Nenhuma pendência.",
            workbook.Worksheet("Pendências").Cell("A2").GetString());
    }

    private static ShoppingListReportDataDto CreateReportData(
        string listName = "Compras para escritório",
        string description = "Materiais mensais",
        string paperItemName = "Papel A4",
        string supplierAName = "Fornecedor A")
    {
        var suppliers = new[]
        {
            new ShoppingListReportSupplierDto(
                SupplierAId,
                supplierAName,
                2,
                0,
                true,
                25m),
            new ShoppingListReportSupplierDto(
                SupplierBId,
                "Fornecedor B",
                1,
                1,
                false,
                16m)
        };

        var items = new[]
        {
            new ShoppingListReportItemDto(
                PaperItemId,
                paperItemName,
                2m,
                "cx",
                8m,
                16m,
                new[]
                {
                    new ShoppingListReportQuoteDto(
                        SupplierAId,
                        supplierAName,
                        10m,
                        20m,
                        false),
                    new ShoppingListReportQuoteDto(
                        SupplierBId,
                        "Fornecedor B",
                        8m,
                        16m,
                        true)
                }),
            new ShoppingListReportItemDto(
                PenItemId,
                "Caneta",
                1m,
                "un",
                5m,
                5m,
                new[]
                {
                    new ShoppingListReportQuoteDto(
                        SupplierAId,
                        supplierAName,
                        5m,
                        5m,
                        true)
                })
        };

        var pendingItems = new[]
        {
            new ShoppingListReportPendingItemDto(
                PenItemId,
                "Caneta",
                new[] { SupplierBId },
                new[] { "Fornecedor B" })
        };

        return new ShoppingListReportDataDto(
            ShoppingListId,
            listName,
            description,
            new DateTime(2026, 7, 1, 12, 0, 0, DateTimeKind.Utc),
            new DateTimeOffset(2026, 7, 25, 18, 30, 0, TimeSpan.Zero),
            new ShoppingListReportSummaryDto(
                2,
                2,
                2,
                3,
                4,
                75m,
                21m,
                true,
                SupplierAId,
                supplierAName,
                25m,
                4m),
            suppliers,
            items,
            pendingItems);
    }

    private static void AssertTextWithoutFormula(
        IXLCell cell,
        string expectedValue)
    {
        Assert.Equal(XLDataType.Text, cell.DataType);
        Assert.False(cell.HasFormula);
        Assert.Equal(expectedValue, cell.GetString());
    }
}
