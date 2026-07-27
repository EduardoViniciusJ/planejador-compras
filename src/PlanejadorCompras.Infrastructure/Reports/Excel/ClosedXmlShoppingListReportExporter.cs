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
    private const string PercentageFormat = "0.00%";
    private const string QuantityFormat = "0.###";
    private const string HeaderColor = "#17365D";
    private const string SectionColor = "#D9EAF7";
    private const string BestPriceColor = "#E2F0D9";
    private const string BestPriceFontColor = "#006100";
    private const string MissingPriceColor = "#FFF2CC";
    private const string MissingPriceFontColor = "#9C6500";
    private const string BorderColor = "#D9E1F2";

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

        BuildQuotesWorksheet(workbook, reportData);
        cancellationToken.ThrowIfCancellationRequested();

        BuildPendingItemsWorksheet(workbook, reportData);
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

        worksheet.Range("A1:B1").Merge();
        SetText(worksheet.Cell("A1"), "Relatório de equalização");
        ApplyTitleStyle(worksheet.Range("A1:B1"));

        SetText(worksheet.Cell("A3"), "Lista");
        SetText(worksheet.Cell("B3"), reportData.Name);
        SetText(worksheet.Cell("A4"), "Descrição");
        SetText(
            worksheet.Cell("B4"),
            string.IsNullOrWhiteSpace(reportData.Description)
                ? "Não informada"
                : reportData.Description);
        worksheet.Cell("B4").Style.Alignment.WrapText = true;

        SetText(worksheet.Cell("A5"), "Criada em");
        SetDateTime(worksheet.Cell("B5"), reportData.CreatedAt);
        SetText(worksheet.Cell("A6"), "Gerado em");
        SetDateTime(worksheet.Cell("B6"), reportData.GeneratedAt.UtcDateTime);

        worksheet.Range("A8:B8").Merge();
        SetText(worksheet.Cell("A8"), "Resumo da equalização");
        ApplySectionStyle(worksheet.Range("A8:B8"));

        SetSummaryValue(worksheet, 9, "Total de itens", reportData.Summary.TotalItems);
        SetSummaryValue(
            worksheet,
            10,
            "Total de fornecedores",
            reportData.Summary.TotalSuppliers);
        SetSummaryValue(
            worksheet,
            11,
            "Itens com ao menos uma cotação",
            reportData.Summary.QuotedItems);
        SetSummaryValue(
            worksheet,
            12,
            "Cotações informadas",
            reportData.Summary.QuotedPriceCount);
        SetSummaryValue(
            worksheet,
            13,
            "Cotações esperadas",
            reportData.Summary.ExpectedPriceCount);

        SetText(worksheet.Cell("A14"), "Cobertura");
        SetPercentage(
            worksheet.Cell("B14"),
            reportData.Summary.CoveragePercentage / 100m);

        SetText(worksheet.Cell("A15"), "Melhor combinação por item");
        SetCurrency(worksheet.Cell("B15"), reportData.Summary.BestChoiceTotal);

        SetText(worksheet.Cell("A16"), "Situação da combinação");
        SetText(
            worksheet.Cell("B16"),
            reportData.Summary.HasCompleteBestChoice ? "Completa" : "Incompleta");
        ApplyStatusStyle(
            worksheet.Cell("B16"),
            reportData.Summary.HasCompleteBestChoice);

        SetText(worksheet.Cell("A17"), "Melhor fornecedor único");
        SetText(
            worksheet.Cell("B17"),
            reportData.Summary.BestCompleteSupplierName ?? "Não disponível");
        ApplyStatusStyle(
            worksheet.Cell("B17"),
            reportData.Summary.BestCompleteSupplierName is not null);

        SetText(worksheet.Cell("A18"), "Total do melhor fornecedor");
        SetOptionalCurrency(
            worksheet.Cell("B18"),
            reportData.Summary.BestCompleteSupplierTotal);

        SetText(worksheet.Cell("A19"), "Economia potencial");
        SetOptionalCurrency(
            worksheet.Cell("B19"),
            reportData.Summary.PotentialSavings);

        const int supplierHeaderRow = 22;
        SetText(worksheet.Cell(supplierHeaderRow, 1), "Fornecedor");
        SetText(worksheet.Cell(supplierHeaderRow, 2), "Itens cotados");
        SetText(worksheet.Cell(supplierHeaderRow, 3), "Itens pendentes");
        SetText(worksheet.Cell(supplierHeaderRow, 4), "Cobertura completa");
        SetText(worksheet.Cell(supplierHeaderRow, 5), "Total cotado");
        ApplyHeaderStyle(worksheet.Range(supplierHeaderRow, 1, supplierHeaderRow, 5));

        var row = supplierHeaderRow + 1;

        if (reportData.Suppliers.Count == 0)
        {
            worksheet.Range(row, 1, row, 5).Merge();
            SetText(worksheet.Cell(row, 1), "Nenhum fornecedor cadastrado.");
            ApplyMissingStyle(worksheet.Range(row, 1, row, 5));
        }
        else
        {
            foreach (var supplier in reportData.Suppliers)
            {
                SetText(worksheet.Cell(row, 1), supplier.Name);
                worksheet.Cell(row, 2).SetValue(supplier.QuotedItemCount);
                worksheet.Cell(row, 3).SetValue(supplier.MissingItemCount);
                SetText(
                    worksheet.Cell(row, 4),
                    supplier.HasCompleteCoverage ? "Sim" : "Não");
                ApplyStatusStyle(
                    worksheet.Cell(row, 4),
                    supplier.HasCompleteCoverage);
                SetCurrency(worksheet.Cell(row, 5), supplier.QuotedTotal);
                row++;
            }
        }

        worksheet.Column(1).Width = 32;
        worksheet.Column(2).Width = 48;
        worksheet.Columns(3, 4).Width = 18;
        worksheet.Column(5).Width = 20;
        worksheet.SheetView.FreezeRows(1);

        ApplyUsedRangeBorders(worksheet);
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

        SetText(worksheet.Cell(1, 1), "Item");
        SetText(worksheet.Cell(1, 2), "Quantidade");
        SetText(worksheet.Cell(1, 3), "Unidade");

        if (suppliers.Count == 0)
        {
            SetText(worksheet.Cell(1, 4), "Situação");
        }
        else
        {
            for (var supplierIndex = 0; supplierIndex < suppliers.Count; supplierIndex++)
            {
                var supplier = suppliers[supplierIndex];
                var unitPriceColumn = 4 + (supplierIndex * 2);

                SetText(
                    worksheet.Cell(1, unitPriceColumn),
                    $"{supplier.Name} - Preço unitário");
                SetText(
                    worksheet.Cell(1, unitPriceColumn + 1),
                    $"{supplier.Name} - Preço total");
            }
        }

        ApplyHeaderStyle(worksheet.Range(1, 1, 1, lastColumn));

        var row = 2;

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

        if (lastItemRow >= 2)
        {
            worksheet.Range(1, 1, lastItemRow, lastColumn).SetAutoFilter();
        }

        worksheet.Column(1).Width = 30;
        worksheet.Column(2).Width = 13;
        worksheet.Column(3).Width = 12;
        worksheet.Columns(4, lastColumn).Width = 20;
        worksheet.SheetView.FreezeRows(1);
        worksheet.SheetView.FreezeColumns(3);

        ApplyUsedRangeBorders(worksheet);
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

    private static void BuildQuotesWorksheet(
        XLWorkbook workbook,
        ShoppingListReportDataDto reportData)
    {
        var worksheet = workbook.Worksheets.Add("Cotações");
        ApplyWorksheetDefaults(worksheet);

        var headers = new[]
        {
            "Item",
            "Quantidade",
            "Unidade",
            "Fornecedor",
            "Preço unitário",
            "Preço total",
            "Melhor preço"
        };

        for (var column = 1; column <= headers.Length; column++)
        {
            SetText(worksheet.Cell(1, column), headers[column - 1]);
        }

        ApplyHeaderStyle(worksheet.Range(1, 1, 1, headers.Length));

        var row = 2;

        foreach (var item in reportData.Items)
        {
            foreach (var quote in item.Quotes)
            {
                SetText(worksheet.Cell(row, 1), item.Name);
                worksheet.Cell(row, 2).SetValue(item.Quantity);
                worksheet.Cell(row, 2).Style.NumberFormat.Format = QuantityFormat;
                SetText(worksheet.Cell(row, 3), item.Unit);
                SetText(worksheet.Cell(row, 4), quote.SupplierName);
                SetCurrency(worksheet.Cell(row, 5), quote.UnitPrice);
                SetCurrency(worksheet.Cell(row, 6), quote.TotalPrice);
                SetText(worksheet.Cell(row, 7), quote.IsLowestPrice ? "Sim" : "Não");

                if (quote.IsLowestPrice)
                {
                    ApplyBestPriceStyle(worksheet.Range(row, 5, row, 7));
                }

                row++;
            }
        }

        var lastQuoteRow = row - 1;

        if (lastQuoteRow < 2)
        {
            worksheet.Range(2, 1, 2, headers.Length).Merge();
            SetText(worksheet.Cell(2, 1), "Nenhuma cotação informada.");
            ApplyMissingStyle(worksheet.Range(2, 1, 2, headers.Length));
        }
        else
        {
            worksheet.Range(1, 1, lastQuoteRow, headers.Length).SetAutoFilter();
        }

        worksheet.Column(1).Width = 30;
        worksheet.Column(2).Width = 13;
        worksheet.Column(3).Width = 12;
        worksheet.Column(4).Width = 28;
        worksheet.Columns(5, 6).Width = 18;
        worksheet.Column(7).Width = 15;
        worksheet.SheetView.FreezeRows(1);

        ApplyUsedRangeBorders(worksheet);
    }

    private static void BuildPendingItemsWorksheet(
        XLWorkbook workbook,
        ShoppingListReportDataDto reportData)
    {
        var worksheet = workbook.Worksheets.Add("Pendências");
        ApplyWorksheetDefaults(worksheet);

        SetText(worksheet.Cell(1, 1), "Item");
        SetText(worksheet.Cell(1, 2), "Fornecedor pendente");
        SetText(worksheet.Cell(1, 3), "Situação");
        ApplyHeaderStyle(worksheet.Range(1, 1, 1, 3));

        var row = 2;

        foreach (var pendingItem in reportData.PendingItems)
        {
            if (pendingItem.MissingSupplierNames.Count > 0)
            {
                foreach (var supplierName in pendingItem.MissingSupplierNames)
                {
                    SetText(worksheet.Cell(row, 1), pendingItem.ItemName);
                    SetText(worksheet.Cell(row, 2), supplierName);
                    SetText(worksheet.Cell(row, 3), "Preço não informado");
                    ApplyMissingStyle(worksheet.Range(row, 1, row, 3));
                    row++;
                }

                continue;
            }

            SetText(worksheet.Cell(row, 1), pendingItem.ItemName);
            SetText(worksheet.Cell(row, 2), "—");
            SetText(
                worksheet.Cell(row, 3),
                reportData.Suppliers.Count == 0
                    ? "Nenhum fornecedor cadastrado"
                    : "Preço pendente");
            ApplyMissingStyle(worksheet.Range(row, 1, row, 3));
            row++;
        }

        var lastPendingRow = row - 1;

        if (lastPendingRow < 2)
        {
            worksheet.Range(2, 1, 2, 3).Merge();
            SetText(worksheet.Cell(2, 1), "Nenhuma pendência.");
            ApplyBestPriceStyle(worksheet.Range(2, 1, 2, 3));
        }
        else
        {
            worksheet.Range(1, 1, lastPendingRow, 3).SetAutoFilter();
        }

        worksheet.Column(1).Width = 30;
        worksheet.Column(2).Width = 30;
        worksheet.Column(3).Width = 30;
        worksheet.SheetView.FreezeRows(1);

        ApplyUsedRangeBorders(worksheet);
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

    private static void ApplyStatusStyle(IXLCell cell, bool isPositive)
    {
        if (isPositive)
        {
            ApplyBestPriceStyle(cell);
            return;
        }

        ApplyMissingStyle(cell);
    }

    private static void ApplyUsedRangeBorders(IXLWorksheet worksheet)
    {
        var usedRange = worksheet.RangeUsed();

        if (usedRange is null)
        {
            return;
        }

        usedRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        usedRange.Style.Border.InsideBorderColor = XLColor.FromHtml(BorderColor);
        usedRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        usedRange.Style.Border.OutsideBorderColor = XLColor.FromHtml(BorderColor);
    }

    private static void SetSummaryValue(
        IXLWorksheet worksheet,
        int row,
        string label,
        int value)
    {
        SetText(worksheet.Cell(row, 1), label);
        worksheet.Cell(row, 2).SetValue(value);
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

    private static void SetPercentage(IXLCell cell, decimal value)
    {
        cell.SetValue(value);
        cell.Style.NumberFormat.Format = PercentageFormat;
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
