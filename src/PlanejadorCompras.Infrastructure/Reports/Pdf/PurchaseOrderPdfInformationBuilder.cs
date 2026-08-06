using PlanejadorCompras.Application.Features.Reports.Contracts;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;

namespace PlanejadorCompras.Infrastructure.Reports.Pdf;

internal static class PurchaseOrderPdfInformationBuilder
{
    internal static void Add(Section section, PurchaseOrderReportDataDto data)
    {
        section.AddParagraph("Informações do pedido", StyleNames.Heading1);

        var table = section.AddTable();
        table.Borders.Width = Unit.FromPoint(0.5);
        table.Borders.Color = OperationalPdfTheme.BorderColor;
        table.AddColumn(Unit.FromCentimeter(4.2));
        table.AddColumn(Unit.FromCentimeter(4.8));
        table.AddColumn(Unit.FromCentimeter(4.2));
        table.AddColumn(Unit.FromCentimeter(4.8));

        var firstRow = table.AddRow();
        AddLabelValue(firstRow.Cells[0], firstRow.Cells[1], "Fornecedor", data.SupplierName);
        AddLabelValue(firstRow.Cells[2], firstRow.Cells[3], "Lista de origem", data.ShoppingListName);

        var secondRow = table.AddRow();
        AddLabelValue(secondRow.Cells[0], secondRow.Cells[1], "Responsável", data.BuyerName);
        AddLabelValue(
            secondRow.Cells[2],
            secondRow.Cells[3],
            "E-mail",
            data.BuyerEmail ?? "Não informado");

        var thirdRow = table.AddRow();
        AddLabelValue(
            thirdRow.Cells[0],
            thirdRow.Cells[1],
            "Previsão de entrega",
            data.ExpectedDeliveryDate?.ToString("dd/MM/yyyy", OperationalPdfTheme.BrazilianCulture)
                ?? "Não informada");
        AddLabelValue(
            thirdRow.Cells[2],
            thirdRow.Cells[3],
            "Condição de pagamento",
            data.PaymentTerms ?? "Não informada");
    }

    private static void AddLabelValue(
        Cell labelCell,
        Cell valueCell,
        string label,
        string value)
    {
        labelCell.Shading.Color = OperationalPdfTheme.HeaderBackground;
        labelCell.Format.Font.Bold = true;
        OperationalPdfTheme.AddCell(labelCell, label, ParagraphAlignment.Left, bold: true);
        OperationalPdfTheme.AddCell(
            valueCell,
            OperationalPdfTheme.LimitText(value, 160),
            ParagraphAlignment.Left);
    }
}
