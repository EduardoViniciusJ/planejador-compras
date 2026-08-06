using ClosedXML.Excel;
using PlanejadorCompras.Application.Features.Reports.Contracts;

namespace PlanejadorCompras.Infrastructure.Reports.Excel;

internal static class ShoppingListPriceMapWorksheetBuilder
{
    internal static void Build(
        XLWorkbook workbook,
        ShoppingListReportDataDto reportData)
    {
        var worksheet = workbook.Worksheets.Add("Mapa de preços");
        ClosedXmlReportStyles.ApplyWorksheetDefaults(worksheet);

        var suppliers = reportData.Suppliers.ToList();
        var lastColumn = suppliers.Count == 0 ? 4 : 3 + (suppliers.Count * 2);
        const int headerRow = 6;
        const int firstItemRow = headerRow + 1;

        worksheet.Range(1, 1, 1, lastColumn).Merge();
        ClosedXmlReportStyles.SetText(worksheet.Cell(1, 1), "Mapa de preços");
        ClosedXmlReportStyles.ApplyTitleStyle(worksheet.Range(1, 1, 1, lastColumn));

        ClosedXmlReportStyles.SetText(worksheet.Cell(3, 1), "Lista");
        worksheet.Range(3, 2, 3, lastColumn).Merge();
        ClosedXmlReportStyles.SetText(worksheet.Cell(3, 2), reportData.Name);
        ClosedXmlReportStyles.SetText(worksheet.Cell(4, 1), "Gerado em");
        worksheet.Range(4, 2, 4, lastColumn).Merge();
        ClosedXmlReportStyles.SetDateTime(
            worksheet.Cell(4, 2),
            reportData.GeneratedAt.UtcDateTime);
        worksheet.Cell(4, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        worksheet.Range(3, 1, 4, 1).Style.Font.Bold = true;

        ClosedXmlReportStyles.SetText(worksheet.Cell(headerRow, 1), "Item");
        ClosedXmlReportStyles.SetText(worksheet.Cell(headerRow, 2), "Quantidade");
        ClosedXmlReportStyles.SetText(worksheet.Cell(headerRow, 3), "Unidade");
        ShoppingListPriceMapRowsWriter.WriteSupplierHeaders(
            worksheet,
            suppliers,
            headerRow);

        ClosedXmlReportStyles.ApplyHeaderStyle(
            worksheet.Range(headerRow, 1, headerRow, lastColumn));
        worksheet.Row(headerRow).Height = 42;

        var row = firstItemRow;
        foreach (var item in reportData.Items)
        {
            ClosedXmlReportStyles.SetText(worksheet.Cell(row, 1), item.Name);
            worksheet.Cell(row, 2).SetValue(item.Quantity);
            worksheet.Cell(row, 2).Style.NumberFormat.Format =
                ClosedXmlReportStyles.QuantityFormat;
            ClosedXmlReportStyles.SetText(worksheet.Cell(row, 3), item.Unit);

            if (suppliers.Count == 0)
            {
                ClosedXmlReportStyles.SetText(
                    worksheet.Cell(row, 4),
                    "Nenhum fornecedor cadastrado");
                ClosedXmlReportStyles.ApplyMissingStyle(worksheet.Cell(row, 4));
            }
            else
            {
                ShoppingListPriceMapRowsWriter.WriteItemSupplierPrices(
                    worksheet,
                    row,
                    item,
                    suppliers);
            }

            row++;
        }

        var lastItemRow = row - 1;
        var totalRow = row;
        ShoppingListPriceMapRowsWriter.WriteTotals(
            worksheet,
            reportData,
            suppliers,
            totalRow);

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
        ClosedXmlReportStyles.ConfigurePrintLayout(worksheet, landscape: true);
        ClosedXmlReportStyles.ApplyRangeBorders(worksheet.Range(3, 1, 4, lastColumn));
        ClosedXmlReportStyles.ApplyRangeBorders(
            worksheet.Range(headerRow, 1, totalRow, lastColumn));
    }

}
