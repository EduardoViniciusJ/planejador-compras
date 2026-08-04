using ClosedXML.Excel;
using PlanejadorCompras.Application.Common.Dtos.Reports;
using PlanejadorCompras.Application.Services.Interfaces;

namespace PlanejadorCompras.Infrastructure.Reports.Excel;

public sealed class ClosedXmlShoppingListReportExporter : IShoppingListExcelExporter
{
    private const int ExcelCellTextLimit = 32_767;
    private const string ContentType =
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    private const string CurrencyFormat = "[$R$-pt-BR] #,##0.00";
    private const string DateTimeFormat = "dd/mm/yyyy hh:mm";
    private const string QuantityFormat = "0.###";
    private const string HeaderColor = "#1F1F1F";
    private const string SectionColor = "#F2F2F2";
    private const string BestPriceColor = "#E2F0D9";
    private const string BestPriceFontColor = "#006100";
    private const string MissingPriceColor = "#FFF2CC";
    private const string MissingPriceFontColor = "#9C6500";
    private const string BorderColor = "#D2D2D2";

    public Task<ExportedFileDto> ExportAsync(
        ShoppingListReportDataDto reportData,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(reportData);
        cancellationToken.ThrowIfCancellationRequested();

        using var workbook = new XLWorkbook();

        BuildSummaryWorksheet(workbook, reportData);
        cancellationToken.ThrowIfCancellationRequested();

        BuildPriceMapWorksheet(workbook, reportData);
        cancellationToken.ThrowIfCancellationRequested();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        cancellationToken.ThrowIfCancellationRequested();

        var exportedFile = new ExportedFileDto(
            stream.ToArray(),
            ContentType,
            ReportFileNameBuilder.BuildEqualizationFileName(
                reportData.Name,
                "xlsx"));

        return Task.FromResult(exportedFile);
    }

    private static void BuildSummaryWorksheet(
        XLWorkbook workbook,
        ShoppingListReportDataDto reportData)
    {
        var worksheet = workbook.Worksheets.Add("Resumo");
        ApplyWorksheetDefaults(worksheet);

        worksheet.Range("A1:D1").Merge();
        SetText(worksheet.Cell("A1"), "Equalização de preços");
        ApplyTitleStyle(worksheet.Range("A1:D1"));

        SetText(worksheet.Cell("A3"), "Lista");
        worksheet.Range("B3:D3").Merge();
        SetText(worksheet.Cell("B3"), reportData.Name);
        SetText(worksheet.Cell("A4"), "Descrição");
        worksheet.Range("B4:D4").Merge();
        SetText(
            worksheet.Cell("B4"),
            string.IsNullOrWhiteSpace(reportData.Description)
                ? "Não informada"
                : reportData.Description);
        worksheet.Cell("B4").Style.Alignment.WrapText = true;

        SetText(worksheet.Cell("A5"), "Gerado em");
        worksheet.Range("B5:D5").Merge();
        SetDateTime(worksheet.Cell("B5"), reportData.GeneratedAt.UtcDateTime);
        worksheet.Cell("B5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        worksheet.Range("A3:A5").Style.Font.Bold = true;

        worksheet.Range("A7:D7").Merge();
        SetText(worksheet.Cell("A7"), "Resultado");
        ApplySectionStyle(worksheet.Range("A7:D7"));

        SetResultRow(
            worksheet,
            8,
            "Menores preços por item",
            reportData.Summary.BestChoiceTotal);
        SetResultRow(
            worksheet,
            9,
            "Melhor fornecedor completo",
            reportData.Summary.BestCompleteSupplierName ?? "Não disponível");
        SetResultRow(
            worksheet,
            10,
            "Total do fornecedor completo",
            reportData.Summary.BestCompleteSupplierTotal);
        SetResultRow(
            worksheet,
            11,
            "Economia estimada",
            reportData.Summary.PotentialSavings);

        worksheet.Column(1).Width = 30;
        worksheet.Columns(2, 3).Width = 18;
        worksheet.Column(4).Width = 22;
        worksheet.SheetView.FreezeRows(1);
        ConfigurePrintLayout(worksheet, landscape: false);
        ApplyRangeBorders(worksheet.Range("A3:D5"));
        ApplyRangeBorders(worksheet.Range("A7:D11"));
    }

    private static void BuildPriceMapWorksheet(
        XLWorkbook workbook,
        ShoppingListReportDataDto reportData)
    {
        var worksheet = workbook.Worksheets.Add("Mapa de preços");
        ApplyWorksheetDefaults(worksheet);

        var suppliers = reportData.Suppliers.ToList();
        var lastColumn = suppliers.Count == 0
            ? 4
            : 3 + (suppliers.Count * 2);
        const int headerRow = 6;
        const int firstItemRow = headerRow + 1;

        worksheet.Range(1, 1, 1, lastColumn).Merge();
        SetText(worksheet.Cell(1, 1), "Mapa de preços");
        ApplyTitleStyle(worksheet.Range(1, 1, 1, lastColumn));

        SetText(worksheet.Cell(3, 1), "Lista");
        worksheet.Range(3, 2, 3, lastColumn).Merge();
        SetText(worksheet.Cell(3, 2), reportData.Name);
        SetText(worksheet.Cell(4, 1), "Gerado em");
        worksheet.Range(4, 2, 4, lastColumn).Merge();
        SetDateTime(worksheet.Cell(4, 2), reportData.GeneratedAt.UtcDateTime);
        worksheet.Cell(4, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        worksheet.Range(3, 1, 4, 1).Style.Font.Bold = true;

        SetText(worksheet.Cell(headerRow, 1), "Item");
        SetText(worksheet.Cell(headerRow, 2), "Quantidade");
        SetText(worksheet.Cell(headerRow, 3), "Unidade");

        if (suppliers.Count == 0)
        {
            SetText(worksheet.Cell(headerRow, 4), "Situação");
        }
        else
        {
            for (var supplierIndex = 0; supplierIndex < suppliers.Count; supplierIndex++)
            {
                var supplier = suppliers[supplierIndex];
                var unitPriceColumn = 4 + (supplierIndex * 2);

                SetText(
                    worksheet.Cell(headerRow, unitPriceColumn),
                    $"{supplier.Name}\nPreço unitário");
                SetText(
                    worksheet.Cell(headerRow, unitPriceColumn + 1),
                    $"{supplier.Name}\nTotal");
            }
        }

        ApplyHeaderStyle(worksheet.Range(headerRow, 1, headerRow, lastColumn));
        worksheet.Row(headerRow).Height = 42;

        var row = firstItemRow;

        foreach (var item in reportData.Items)
        {
            SetText(worksheet.Cell(row, 1), item.Name);
            worksheet.Cell(row, 2).SetValue(item.Quantity);
            worksheet.Cell(row, 2).Style.NumberFormat.Format = QuantityFormat;
            SetText(worksheet.Cell(row, 3), item.Unit);

            if (suppliers.Count == 0)
            {
                SetText(worksheet.Cell(row, 4), "Nenhum fornecedor cadastrado");
                ApplyMissingStyle(worksheet.Cell(row, 4));
            }
            else
            {
                WriteItemSupplierPrices(worksheet, row, item, suppliers);
            }

            row++;
        }

        var lastItemRow = row - 1;
        var totalRow = row;
        SetText(worksheet.Cell(totalRow, 1), "Total cotado");
        worksheet.Range(totalRow, 1, totalRow, 3).Merge();
        worksheet.Cell(totalRow, 1).Style.Font.Bold = true;

        if (suppliers.Count == 0)
        {
            SetText(worksheet.Cell(totalRow, 4), "Sem preços");
            ApplyMissingStyle(worksheet.Cell(totalRow, 4));
        }
        else
        {
            for (var supplierIndex = 0; supplierIndex < suppliers.Count; supplierIndex++)
            {
                var supplier = suppliers[supplierIndex];
                var unitPriceColumn = 4 + (supplierIndex * 2);
                var totalPriceColumn = unitPriceColumn + 1;

                SetText(
                    worksheet.Cell(totalRow, unitPriceColumn),
                    supplier.HasCompleteCoverage
                        ? "Completo"
                        : $"{supplier.MissingItemCount} pendente(s)");
                SetCurrency(
                    worksheet.Cell(totalRow, totalPriceColumn),
                    supplier.QuotedTotal);

                if (supplier.SupplierId == reportData.Summary.BestCompleteSupplierId)
                {
                    ApplyBestPriceStyle(
                        worksheet.Range(
                            totalRow,
                            unitPriceColumn,
                            totalRow,
                            totalPriceColumn));
                }
                else if (!supplier.HasCompleteCoverage)
                {
                    ApplyMissingStyle(
                        worksheet.Range(
                            totalRow,
                            unitPriceColumn,
                            totalRow,
                            totalPriceColumn));
                }
            }
        }

        worksheet.Range(totalRow, 1, totalRow, lastColumn)
            .Style.Border.TopBorder = XLBorderStyleValues.Double;

        if (lastItemRow >= firstItemRow)
        {
            worksheet.Range(headerRow, 1, lastItemRow, lastColumn).SetAutoFilter();
        }

        worksheet.Column(1).Width = 30;
        worksheet.Column(2).Width = 13;
        worksheet.Column(3).Width = 12;
        worksheet.Columns(4, lastColumn).Width = 20;
        worksheet.SheetView.FreezeRows(headerRow);
        worksheet.SheetView.FreezeColumns(3);

        ConfigurePrintLayout(worksheet, landscape: true);
        ApplyRangeBorders(worksheet.Range(3, 1, 4, lastColumn));
        ApplyRangeBorders(worksheet.Range(headerRow, 1, totalRow, lastColumn));
    }

    private static void WriteItemSupplierPrices(
        IXLWorksheet worksheet,
        int row,
        ShoppingListReportItemDto item,
        IReadOnlyList<ShoppingListReportSupplierDto> suppliers)
    {
        var quotesBySupplier = item.Quotes.ToDictionary(quote => quote.SupplierId);

        for (var supplierIndex = 0; supplierIndex < suppliers.Count; supplierIndex++)
        {
            var supplier = suppliers[supplierIndex];
            var unitPriceColumn = 4 + (supplierIndex * 2);
            var totalPriceColumn = unitPriceColumn + 1;

            if (!quotesBySupplier.TryGetValue(supplier.SupplierId, out var quote))
            {
                SetText(worksheet.Cell(row, unitPriceColumn), "Sem preço");
                SetText(worksheet.Cell(row, totalPriceColumn), "Sem preço");
                ApplyMissingStyle(
                    worksheet.Range(
                        row,
                        unitPriceColumn,
                        row,
                        totalPriceColumn));
                continue;
            }

            SetCurrency(worksheet.Cell(row, unitPriceColumn), quote.UnitPrice);
            SetCurrency(worksheet.Cell(row, totalPriceColumn), quote.TotalPrice);

            if (quote.IsLowestPrice)
            {
                ApplyBestPriceStyle(
                    worksheet.Range(
                        row,
                        unitPriceColumn,
                        row,
                        totalPriceColumn));
            }
        }
    }

    private static void ApplyWorksheetDefaults(IXLWorksheet worksheet)
    {
        worksheet.ShowGridLines = false;
        worksheet.Style.Font.FontName = "Aptos";
        worksheet.Style.Font.FontSize = 10;
        worksheet.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        worksheet.RowHeight = 20;
    }

    private static void ApplyTitleStyle(IXLRange range)
    {
        range.Style.Fill.BackgroundColor = XLColor.FromHtml(HeaderColor);
        range.Style.Font.FontColor = XLColor.White;
        range.Style.Font.FontSize = 18;
        range.Style.Font.Bold = true;
        range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        range.Worksheet
            .Row(range.RangeAddress.FirstAddress.RowNumber)
            .Height = 32;
    }

    private static void ApplySectionStyle(IXLRange range)
    {
        range.Style.Fill.BackgroundColor = XLColor.FromHtml(SectionColor);
        range.Style.Font.FontColor = XLColor.FromHtml(HeaderColor);
        range.Style.Font.Bold = true;
    }

    private static void ApplyHeaderStyle(IXLRange range)
    {
        range.Style.Fill.BackgroundColor = XLColor.FromHtml(HeaderColor);
        range.Style.Font.FontColor = XLColor.White;
        range.Style.Font.Bold = true;
        range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        range.Style.Alignment.WrapText = true;
        range.Worksheet
            .Row(range.RangeAddress.FirstAddress.RowNumber)
            .Height = 34;
    }

    private static void ApplyBestPriceStyle(IXLCell cell)
    {
        cell.Style.Fill.BackgroundColor = XLColor.FromHtml(BestPriceColor);
        cell.Style.Font.FontColor = XLColor.FromHtml(BestPriceFontColor);
        cell.Style.Font.Bold = true;
    }

    private static void ApplyBestPriceStyle(IXLRange range)
    {
        range.Style.Fill.BackgroundColor = XLColor.FromHtml(BestPriceColor);
        range.Style.Font.FontColor = XLColor.FromHtml(BestPriceFontColor);
        range.Style.Font.Bold = true;
    }

    private static void ApplyMissingStyle(IXLCell cell)
    {
        cell.Style.Fill.BackgroundColor = XLColor.FromHtml(MissingPriceColor);
        cell.Style.Font.FontColor = XLColor.FromHtml(MissingPriceFontColor);
    }

    private static void ApplyMissingStyle(IXLRange range)
    {
        range.Style.Fill.BackgroundColor = XLColor.FromHtml(MissingPriceColor);
        range.Style.Font.FontColor = XLColor.FromHtml(MissingPriceFontColor);
    }

    private static void ApplyRangeBorders(IXLRange range)
    {
        range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        range.Style.Border.OutsideBorderColor = XLColor.FromHtml(BorderColor);

        foreach (var row in range.Rows())
        {
            row.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            row.Style.Border.BottomBorderColor = XLColor.FromHtml(BorderColor);
        }
    }

    private static void SetResultRow(
        IXLWorksheet worksheet,
        int row,
        string label,
        object? value)
    {
        worksheet.Range(row, 1, row, 3).Merge();
        SetText(worksheet.Cell(row, 1), label);
        worksheet.Cell(row, 1).Style.Font.Bold = true;

        if (value is decimal currency)
        {
            SetCurrency(worksheet.Cell(row, 4), currency);
        }
        else if (value is null)
        {
            SetOptionalCurrency(worksheet.Cell(row, 4), null);
        }
        else
        {
            SetText(worksheet.Cell(row, 4), value?.ToString() ?? "Não disponível");
        }
    }

    private static void SetOptionalCurrency(IXLCell cell, decimal? value)
    {
        if (value.HasValue)
        {
            SetCurrency(cell, value.Value);
            return;
        }

        SetText(cell, "Não disponível");
        ApplyMissingStyle(cell);
    }

    private static void SetCurrency(IXLCell cell, decimal value)
    {
        cell.SetValue(value);
        cell.Style.NumberFormat.Format = CurrencyFormat;
    }

    private static void ConfigurePrintLayout(IXLWorksheet worksheet, bool landscape)
    {
        worksheet.PageSetup.PageOrientation = landscape
            ? XLPageOrientation.Landscape
            : XLPageOrientation.Portrait;
        worksheet.PageSetup.PaperSize = XLPaperSize.A4Paper;
        worksheet.PageSetup.PagesWide = 1;
        worksheet.PageSetup.PagesTall = 0;
        worksheet.PageSetup.Margins.Top = 0.5;
        worksheet.PageSetup.Margins.Bottom = 0.5;
        worksheet.PageSetup.Margins.Left = 0.4;
        worksheet.PageSetup.Margins.Right = 0.4;
    }

    private static void SetDateTime(IXLCell cell, DateTime value)
    {
        cell.SetValue(value);
        cell.Style.NumberFormat.Format = DateTimeFormat;
    }

    private static void SetText(IXLCell cell, string? value)
    {
        var text = value ?? string.Empty;

        if (text.Length > ExcelCellTextLimit)
        {
            text = text[..ExcelCellTextLimit];
        }

        cell.SetValue(text);
    }
}
