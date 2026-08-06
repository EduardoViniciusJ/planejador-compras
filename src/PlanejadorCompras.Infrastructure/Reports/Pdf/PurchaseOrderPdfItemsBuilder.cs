using PlanejadorCompras.Application.Features.Reports.Contracts;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;

namespace PlanejadorCompras.Infrastructure.Reports.Pdf;

internal static class PurchaseOrderPdfItemsBuilder
{
    internal static void Add(Section section, PurchaseOrderReportDataDto data)
    {
        section.AddParagraph("Itens", StyleNames.Heading1);

        var table = section.AddTable();
        table.Borders.Width = Unit.FromPoint(0.5);
        table.Borders.Color = OperationalPdfTheme.BorderColor;
        table.AddColumn(Unit.FromCentimeter(7.2));
        table.AddColumn(Unit.FromCentimeter(2));
        table.AddColumn(Unit.FromCentimeter(1.6));
        table.AddColumn(Unit.FromCentimeter(3.4));
        table.AddColumn(Unit.FromCentimeter(3.8));

        AddHeader(table);
        foreach (var item in data.Items)
        {
            AddItem(table, item);
        }

        AddTotal(table, data.TotalPrice);
    }

    private static void AddHeader(Table table)
    {
        var header = table.AddRow();
        header.HeadingFormat = true;
        header.Shading.Color = OperationalPdfTheme.HeaderBackground;
        header.Format.Font.Bold = true;
        header.VerticalAlignment = VerticalAlignment.Center;
        OperationalPdfTheme.AddCell(header.Cells[0], "Item", ParagraphAlignment.Left);
        OperationalPdfTheme.AddCell(header.Cells[1], "Quantidade");
        OperationalPdfTheme.AddCell(header.Cells[2], "Un.");
        OperationalPdfTheme.AddCell(header.Cells[3], "Valor unitário", ParagraphAlignment.Right);
        OperationalPdfTheme.AddCell(header.Cells[4], "Total", ParagraphAlignment.Right);
    }

    private static void AddItem(Table table, PurchaseOrderReportItemDto item)
    {
        var row = table.AddRow();
        row.VerticalAlignment = VerticalAlignment.Center;
        row.TopPadding = Unit.FromPoint(4);
        row.BottomPadding = Unit.FromPoint(4);
        OperationalPdfTheme.AddCell(
            row.Cells[0],
            OperationalPdfTheme.LimitText(item.Name, 120),
            ParagraphAlignment.Left);
        OperationalPdfTheme.AddCell(
            row.Cells[1],
            item.Quantity.ToString("0.###", OperationalPdfTheme.BrazilianCulture));
        OperationalPdfTheme.AddCell(
            row.Cells[2],
            OperationalPdfTheme.LimitText(item.Unit, 20));
        OperationalPdfTheme.AddCell(
            row.Cells[3],
            OperationalPdfTheme.FormatCurrency(item.UnitPrice),
            ParagraphAlignment.Right);
        OperationalPdfTheme.AddCell(
            row.Cells[4],
            OperationalPdfTheme.FormatCurrency(item.TotalPrice),
            ParagraphAlignment.Right,
            bold: true);
    }

    private static void AddTotal(Table table, decimal totalPrice)
    {
        var totalRow = table.AddRow();
        totalRow.Borders.Top.Width = Unit.FromPoint(1);
        totalRow.Format.Font.Bold = true;
        totalRow.Cells[0].MergeRight = 3;
        OperationalPdfTheme.AddCell(
            totalRow.Cells[0],
            "Total do pedido",
            ParagraphAlignment.Right);
        OperationalPdfTheme.AddCell(
            totalRow.Cells[4],
            OperationalPdfTheme.FormatCurrency(totalPrice),
            ParagraphAlignment.Right,
            bold: true);
    }
}
