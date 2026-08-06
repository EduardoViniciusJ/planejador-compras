using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using PlanejadorCompras.Application.Features.Reports.Contracts;

namespace PlanejadorCompras.Infrastructure.Reports.Pdf;

internal static class ShoppingListPriceMapTableBuilder
{
    internal static void Add(
        Section section,
        ShoppingListReportDataDto reportData,
        IReadOnlyList<ShoppingListReportSupplierDto> suppliers)
    {
        var table = section.AddTable();
        table.Borders.Width = Unit.FromPoint(0.35);
        table.Borders.Color = ShoppingListPdfTheme.BorderBlue;
        table.Format.Font.Size = Unit.FromPoint(7.2);
        table.AddColumn(Unit.FromCentimeter(5.5));
        table.AddColumn(Unit.FromCentimeter(1.4));
        table.AddColumn(Unit.FromCentimeter(1.3));

        if (suppliers.Count == 0)
        {
            table.AddColumn(Unit.FromCentimeter(18.5));
        }
        else
        {
            var supplierColumnWidth = 18.4 / suppliers.Count;
            foreach (var _ in suppliers)
            {
                table.AddColumn(Unit.FromCentimeter(supplierColumnWidth));
            }
        }

        AddHeader(table, suppliers);

        var itemIndex = 0;
        foreach (var item in reportData.Items)
        {
            var row = table.AddRow();
            row.VerticalAlignment = VerticalAlignment.Center;
            row.TopPadding = Unit.FromPoint(3);
            row.BottomPadding = Unit.FromPoint(3);

            if (itemIndex % 2 == 1)
            {
                row.Shading.Color = ShoppingListPdfTheme.LightBlue;
            }

            ShoppingListPriceMapCellWriter.AddText(
                row.Cells[0],
                ShoppingListPdfTheme.LimitText(
                    item.Name,
                    ShoppingListPdfTheme.TableTextLimit),
                ParagraphAlignment.Left);
            ShoppingListPriceMapCellWriter.AddText(
                row.Cells[1],
                item.Quantity.ToString("0.###", ShoppingListPdfTheme.BrazilianCulture));
            ShoppingListPriceMapCellWriter.AddText(
                row.Cells[2],
                ShoppingListPdfTheme.LimitText(item.Unit, 16));

            if (suppliers.Count == 0)
            {
                ShoppingListPriceMapCellWriter.AddMissingPrice(
                    row.Cells[3],
                    "Nenhum fornecedor cadastrado");
            }
            else
            {
                ShoppingListPriceMapCellWriter.AddSupplierPrices(
                    row,
                    item,
                    suppliers);
            }

            itemIndex++;
        }

        ShoppingListPriceMapCellWriter.AddSupplierTotals(
            table,
            reportData,
            suppliers);
    }

    private static void AddHeader(
        Table table,
        IReadOnlyList<ShoppingListReportSupplierDto> suppliers)
    {
        var header = table.AddRow();
        header.HeadingFormat = true;
        header.VerticalAlignment = VerticalAlignment.Center;
        header.Shading.Color = ShoppingListPdfTheme.DarkBlue;
        header.Format.Font.Bold = true;
        header.Format.Font.Color = Colors.White;
        header.Format.Alignment = ParagraphAlignment.Center;
        header.TopPadding = Unit.FromPoint(4);
        header.BottomPadding = Unit.FromPoint(4);

        ShoppingListPriceMapCellWriter.AddText(
            header.Cells[0],
            "Item",
            ParagraphAlignment.Left);
        ShoppingListPriceMapCellWriter.AddText(header.Cells[1], "Qtd.");
        ShoppingListPriceMapCellWriter.AddText(header.Cells[2], "Un.");

        if (suppliers.Count == 0)
        {
            ShoppingListPriceMapCellWriter.AddText(header.Cells[3], "Situação");
            return;
        }

        for (var index = 0; index < suppliers.Count; index++)
        {
            ShoppingListPriceMapCellWriter.AddText(
                header.Cells[index + 3],
                ShoppingListPdfTheme.LimitText(suppliers[index].Name, 42));
        }
    }

}
