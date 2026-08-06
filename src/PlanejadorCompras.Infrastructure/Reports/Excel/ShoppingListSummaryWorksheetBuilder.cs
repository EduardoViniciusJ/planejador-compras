using ClosedXML.Excel;
using PlanejadorCompras.Application.Features.Reports.Contracts;

namespace PlanejadorCompras.Infrastructure.Reports.Excel;

internal static class ShoppingListSummaryWorksheetBuilder
{
    internal static void Build(
        XLWorkbook workbook,
        ShoppingListReportDataDto reportData)
    {
        var worksheet = workbook.Worksheets.Add("Resumo");
        ClosedXmlReportStyles.ApplyWorksheetDefaults(worksheet);

        worksheet.Range("A1:D1").Merge();
        ClosedXmlReportStyles.SetText(worksheet.Cell("A1"), "Equalização de preços");
        ClosedXmlReportStyles.ApplyTitleStyle(worksheet.Range("A1:D1"));

        ClosedXmlReportStyles.SetText(worksheet.Cell("A3"), "Lista");
        worksheet.Range("B3:D3").Merge();
        ClosedXmlReportStyles.SetText(worksheet.Cell("B3"), reportData.Name);
        ClosedXmlReportStyles.SetText(worksheet.Cell("A4"), "Descrição");
        worksheet.Range("B4:D4").Merge();
        ClosedXmlReportStyles.SetText(
            worksheet.Cell("B4"),
            string.IsNullOrWhiteSpace(reportData.Description)
                ? "Não informada"
                : reportData.Description);
        worksheet.Cell("B4").Style.Alignment.WrapText = true;

        ClosedXmlReportStyles.SetText(worksheet.Cell("A5"), "Gerado em");
        worksheet.Range("B5:D5").Merge();
        ClosedXmlReportStyles.SetDateTime(
            worksheet.Cell("B5"),
            reportData.GeneratedAt.UtcDateTime);
        worksheet.Cell("B5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        worksheet.Range("A3:A5").Style.Font.Bold = true;

        worksheet.Range("A7:D7").Merge();
        ClosedXmlReportStyles.SetText(worksheet.Cell("A7"), "Resultado");
        ClosedXmlReportStyles.ApplySectionStyle(worksheet.Range("A7:D7"));

        SetResultRow(worksheet, 8, "Menores preços por item", reportData.Summary.BestChoiceTotal);
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
        SetResultRow(worksheet, 11, "Economia estimada", reportData.Summary.PotentialSavings);

        worksheet.Column(1).Width = 30;
        worksheet.Columns(2, 3).Width = 18;
        worksheet.Column(4).Width = 22;
        worksheet.SheetView.FreezeRows(1);
        ClosedXmlReportStyles.ConfigurePrintLayout(worksheet, landscape: false);
        ClosedXmlReportStyles.ApplyRangeBorders(worksheet.Range("A3:D5"));
        ClosedXmlReportStyles.ApplyRangeBorders(worksheet.Range("A7:D11"));
    }

    private static void SetResultRow(
        IXLWorksheet worksheet,
        int row,
        string label,
        object? value)
    {
        worksheet.Range(row, 1, row, 3).Merge();
        ClosedXmlReportStyles.SetText(worksheet.Cell(row, 1), label);
        worksheet.Cell(row, 1).Style.Font.Bold = true;

        if (value is decimal currency)
        {
            ClosedXmlReportStyles.SetCurrency(worksheet.Cell(row, 4), currency);
        }
        else if (value is null)
        {
            SetOptionalCurrency(worksheet.Cell(row, 4), null);
        }
        else
        {
            ClosedXmlReportStyles.SetText(
                worksheet.Cell(row, 4),
                value.ToString() ?? "Não disponível");
        }
    }

    private static void SetOptionalCurrency(IXLCell cell, decimal? value)
    {
        if (value.HasValue)
        {
            ClosedXmlReportStyles.SetCurrency(cell, value.Value);
            return;
        }

        ClosedXmlReportStyles.SetText(cell, "Não disponível");
        ClosedXmlReportStyles.ApplyMissingStyle(cell);
    }
}
