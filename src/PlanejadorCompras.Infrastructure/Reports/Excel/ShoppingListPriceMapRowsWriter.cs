using ClosedXML.Excel;
using PlanejadorCompras.Application.Features.Reports.Contracts;

namespace PlanejadorCompras.Infrastructure.Reports.Excel;

internal static class ShoppingListPriceMapRowsWriter
{
    internal static void WriteSupplierHeaders(
        IXLWorksheet worksheet,
        IReadOnlyList<ShoppingListReportSupplierDto> suppliers,
        int headerRow)
    {
        if (suppliers.Count == 0)
        {
            ClosedXmlReportStyles.SetText(worksheet.Cell(headerRow, 4), "Situação");
            return;
        }

        for (var index = 0; index < suppliers.Count; index++)
        {
            var supplier = suppliers[index];
            var unitPriceColumn = 4 + (index * 2);
            ClosedXmlReportStyles.SetText(
                worksheet.Cell(headerRow, unitPriceColumn),
                $"{supplier.Name}\nPreço unitário");
            ClosedXmlReportStyles.SetText(
                worksheet.Cell(headerRow, unitPriceColumn + 1),
                $"{supplier.Name}\nTotal");
        }
    }

    internal static void WriteItemSupplierPrices(
        IXLWorksheet worksheet,
        int row,
        ShoppingListReportItemDto item,
        IReadOnlyList<ShoppingListReportSupplierDto> suppliers)
    {
        var quotesBySupplier = item.Quotes.ToDictionary(quote => quote.SupplierId);

        for (var index = 0; index < suppliers.Count; index++)
        {
            var supplier = suppliers[index];
            var unitPriceColumn = 4 + (index * 2);
            var totalPriceColumn = unitPriceColumn + 1;

            if (!quotesBySupplier.TryGetValue(supplier.SupplierId, out var quote))
            {
                ClosedXmlReportStyles.SetText(
                    worksheet.Cell(row, unitPriceColumn),
                    "Sem preço");
                ClosedXmlReportStyles.SetText(
                    worksheet.Cell(row, totalPriceColumn),
                    "Sem preço");
                ClosedXmlReportStyles.ApplyMissingStyle(
                    worksheet.Range(row, unitPriceColumn, row, totalPriceColumn));
                continue;
            }

            ClosedXmlReportStyles.SetCurrency(
                worksheet.Cell(row, unitPriceColumn),
                quote.UnitPrice);
            ClosedXmlReportStyles.SetCurrency(
                worksheet.Cell(row, totalPriceColumn),
                quote.TotalPrice);

            if (quote.IsLowestPrice)
            {
                ClosedXmlReportStyles.ApplyBestPriceStyle(
                    worksheet.Range(row, unitPriceColumn, row, totalPriceColumn));
            }
        }
    }

    internal static void WriteTotals(
        IXLWorksheet worksheet,
        ShoppingListReportDataDto reportData,
        IReadOnlyList<ShoppingListReportSupplierDto> suppliers,
        int totalRow)
    {
        ClosedXmlReportStyles.SetText(worksheet.Cell(totalRow, 1), "Total cotado");
        worksheet.Range(totalRow, 1, totalRow, 3).Merge();
        worksheet.Cell(totalRow, 1).Style.Font.Bold = true;

        if (suppliers.Count == 0)
        {
            ClosedXmlReportStyles.SetText(worksheet.Cell(totalRow, 4), "Sem preços");
            ClosedXmlReportStyles.ApplyMissingStyle(worksheet.Cell(totalRow, 4));
            return;
        }

        for (var index = 0; index < suppliers.Count; index++)
        {
            WriteSupplierTotal(worksheet, reportData, suppliers[index], totalRow, index);
        }
    }

    private static void WriteSupplierTotal(
        IXLWorksheet worksheet,
        ShoppingListReportDataDto reportData,
        ShoppingListReportSupplierDto supplier,
        int totalRow,
        int supplierIndex)
    {
        var unitPriceColumn = 4 + (supplierIndex * 2);
        var totalPriceColumn = unitPriceColumn + 1;
        ClosedXmlReportStyles.SetText(
            worksheet.Cell(totalRow, unitPriceColumn),
            supplier.HasCompleteCoverage
                ? "Completo"
                : $"{supplier.MissingItemCount} pendente(s)");
        ClosedXmlReportStyles.SetCurrency(
            worksheet.Cell(totalRow, totalPriceColumn),
            supplier.QuotedTotal);

        var range = worksheet.Range(
            totalRow,
            unitPriceColumn,
            totalRow,
            totalPriceColumn);
        if (supplier.SupplierId == reportData.Summary.BestCompleteSupplierId)
        {
            ClosedXmlReportStyles.ApplyBestPriceStyle(range);
        }
        else if (!supplier.HasCompleteCoverage)
        {
            ClosedXmlReportStyles.ApplyMissingStyle(range);
        }
    }
}
