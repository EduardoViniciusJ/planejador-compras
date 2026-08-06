using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using PlanejadorCompras.Application.Features.Reports.Contracts;

namespace PlanejadorCompras.Infrastructure.Reports.Pdf;

internal static class ShoppingListPriceMapCellWriter
{
    internal static void AddSupplierPrices(
        Row row,
        ShoppingListReportItemDto item,
        IReadOnlyList<ShoppingListReportSupplierDto> suppliers)
    {
        var quotesBySupplier = item.Quotes.ToDictionary(quote => quote.SupplierId);

        for (var index = 0; index < suppliers.Count; index++)
        {
            var supplier = suppliers[index];
            var cell = row.Cells[index + 3];

            if (!quotesBySupplier.TryGetValue(supplier.SupplierId, out var quote))
            {
                AddMissingPrice(cell, "Preço não informado");
                continue;
            }

            cell.Format.Alignment = ParagraphAlignment.Center;
            var unitPrice = cell.AddParagraph();
            unitPrice.Format.Font.Bold = true;
            unitPrice.AddText(ShoppingListPdfTheme.FormatCurrency(quote.UnitPrice));

            var totalPrice = cell.AddParagraph();
            totalPrice.Format.Font.Size = Unit.FromPoint(6.7);
            totalPrice.AddText(
                $"Total: {ShoppingListPdfTheme.FormatCurrency(quote.TotalPrice)}");

            if (quote.IsLowestPrice)
            {
                AddBestPriceStyle(cell);
            }
        }
    }

    internal static void AddSupplierTotals(
        Table table,
        ShoppingListReportDataDto reportData,
        IReadOnlyList<ShoppingListReportSupplierDto> suppliers)
    {
        var totalRow = table.AddRow();
        totalRow.VerticalAlignment = VerticalAlignment.Center;
        totalRow.Format.Font.Bold = true;
        totalRow.TopPadding = Unit.FromPoint(4);
        totalRow.BottomPadding = Unit.FromPoint(4);
        totalRow.Borders.Top.Width = Unit.FromPoint(0.9);
        totalRow.Cells[0].MergeRight = 2;
        AddText(
            totalRow.Cells[0],
            "Total cotado por fornecedor",
            ParagraphAlignment.Left);

        if (suppliers.Count == 0)
        {
            AddMissingPrice(totalRow.Cells[3], "Sem preços");
            return;
        }

        for (var index = 0; index < suppliers.Count; index++)
        {
            AddSupplierTotal(
                totalRow.Cells[index + 3],
                suppliers[index],
                reportData.Summary.BestCompleteSupplierId);
        }
    }

    internal static void AddMissingPrice(Cell cell, string message)
    {
        cell.Shading.Color = ShoppingListPdfTheme.MissingPriceBackground;
        cell.Format.Font.Color = ShoppingListPdfTheme.MissingPriceForeground;
        cell.Format.Alignment = ParagraphAlignment.Center;

        var paragraph = cell.AddParagraph();
        paragraph.Format.Font.Size = Unit.FromPoint(6.8);
        paragraph.AddText(message);
    }

    internal static void AddText(
        Cell cell,
        string text,
        ParagraphAlignment alignment = ParagraphAlignment.Center)
    {
        cell.Format.Alignment = alignment;
        cell.AddParagraph(text);
    }

    private static void AddBestPriceStyle(Cell cell)
    {
        cell.Shading.Color = ShoppingListPdfTheme.BestPriceBackground;
        cell.Format.Font.Color = ShoppingListPdfTheme.BestPriceForeground;

        var bestPrice = cell.AddParagraph();
        bestPrice.Format.Font.Size = Unit.FromPoint(6.2);
        bestPrice.Format.Font.Bold = true;
        bestPrice.AddText("Melhor preço");
    }

    private static void AddSupplierTotal(
        Cell cell,
        ShoppingListReportSupplierDto supplier,
        Guid? bestCompleteSupplierId)
    {
        cell.Format.Alignment = ParagraphAlignment.Center;

        var total = cell.AddParagraph();
        total.AddText(ShoppingListPdfTheme.FormatCurrency(supplier.QuotedTotal));

        var status = cell.AddParagraph();
        status.Format.Font.Size = Unit.FromPoint(6.3);
        status.AddText(
            supplier.HasCompleteCoverage
                ? "Cobertura completa"
                : $"{supplier.MissingItemCount} pendente(s)");

        if (supplier.SupplierId == bestCompleteSupplierId)
        {
            cell.Shading.Color = ShoppingListPdfTheme.BestPriceBackground;
            cell.Format.Font.Color = ShoppingListPdfTheme.BestPriceForeground;
        }
        else if (!supplier.HasCompleteCoverage)
        {
            cell.Shading.Color = ShoppingListPdfTheme.MissingPriceBackground;
            cell.Format.Font.Color = ShoppingListPdfTheme.MissingPriceForeground;
        }
    }
}
