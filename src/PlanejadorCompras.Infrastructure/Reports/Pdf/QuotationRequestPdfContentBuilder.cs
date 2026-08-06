using PlanejadorCompras.Application.Features.Reports.Contracts;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;

namespace PlanejadorCompras.Infrastructure.Reports.Pdf;

internal static class QuotationRequestPdfContentBuilder
{
    internal static void AddContent(Section section, QuotationRequestReportDataDto data)
    {
        AddHeader(section, data);
        AddMetadata(section, data);
        AddItems(section, data.Items);
        AddRequestedInformation(section);
        AddOptionalDetails(section, data);
    }

    private static void AddHeader(Section section, QuotationRequestReportDataDto data)
    {
        var title = section.AddParagraph("Solicitação de cotação");
        title.Format.Font.Size = Unit.FromPoint(20);
        title.Format.Font.Bold = true;
        title.Format.SpaceAfter = Unit.FromPoint(3);

        var identification = section.AddParagraph();
        identification.Format.Font.Size = Unit.FromPoint(10);
        identification.Format.Font.Color = OperationalPdfTheme.MutedColor;
        identification.Format.SpaceAfter = Unit.FromPoint(10);
        identification.AddFormattedText(data.Code, TextFormat.Bold);
        identification.AddText($" | Emitida em {FormatDate(data.IssuedOn)}");
        identification.AddText(
            data.ResponseDeadline.HasValue
                ? $" | Responder até {FormatDate(data.ResponseDeadline.Value)}"
                : " | Prazo de resposta não informado");
    }

    private static void AddMetadata(Section section, QuotationRequestReportDataDto data)
    {
        section.AddParagraph("Informações da solicitação", StyleNames.Heading1);

        var table = section.AddTable();
        table.Borders.Width = Unit.FromPoint(0.5);
        table.Borders.Color = OperationalPdfTheme.BorderColor;
        table.AddColumn(Unit.FromCentimeter(4.2));
        table.AddColumn(Unit.FromCentimeter(12.6));

        AddMetadataRow(table, "Lista de compras", data.ShoppingListName);
        AddMetadataRow(table, "Responsável", data.BuyerName);
        AddMetadataRow(
            table,
            "Contato",
            string.IsNullOrWhiteSpace(data.BuyerEmail) ? "Não informado" : data.BuyerEmail);

        if (!string.IsNullOrWhiteSpace(data.Description))
        {
            AddMetadataRow(table, "Descrição", data.Description);
        }

        if (!string.IsNullOrWhiteSpace(data.DeliveryAddress))
        {
            AddMetadataRow(table, "Local de entrega", data.DeliveryAddress);
        }

        section.AddParagraph().Format.SpaceAfter = Unit.FromPoint(5);
    }

    private static void AddMetadataRow(Table table, string label, string value)
    {
        var row = table.AddRow();
        row.TopPadding = Unit.FromPoint(4);
        row.BottomPadding = Unit.FromPoint(4);
        row.Cells[0].Shading.Color = OperationalPdfTheme.HeaderBackground;
        row.Cells[0].AddParagraph(label).Format.Font.Bold = true;
        row.Cells[1].AddParagraph(value);
    }

    private static void AddItems(
        Section section,
        IReadOnlyCollection<QuotationRequestReportItemDto> items)
    {
        AddSectionTitle(section, "Itens solicitados");

        var table = section.AddTable();
        table.Borders.Width = Unit.FromPoint(0.5);
        table.Borders.Color = OperationalPdfTheme.BorderColor;
        table.AddColumn(Unit.FromCentimeter(11.5));
        table.AddColumn(Unit.FromCentimeter(2.6));
        table.AddColumn(Unit.FromCentimeter(2.7));

        var header = table.AddRow();
        header.HeadingFormat = true;
        header.Shading.Color = OperationalPdfTheme.HeaderBackground;
        header.Format.Font.Bold = true;
        OperationalPdfTheme.AddCell(header.Cells[0], "Item", ParagraphAlignment.Left);
        OperationalPdfTheme.AddCell(header.Cells[1], "Quantidade");
        OperationalPdfTheme.AddCell(header.Cells[2], "Unidade");

        foreach (var item in items)
        {
            var row = table.AddRow();
            row.TopPadding = Unit.FromPoint(4);
            row.BottomPadding = Unit.FromPoint(4);
            OperationalPdfTheme.AddCell(row.Cells[0], item.Name, ParagraphAlignment.Left);
            OperationalPdfTheme.AddCell(
                row.Cells[1],
                item.Quantity.ToString("0.###", OperationalPdfTheme.BrazilianCulture));
            OperationalPdfTheme.AddCell(row.Cells[2], item.Unit);
        }
    }

    private static void AddRequestedInformation(Section section)
    {
        AddSectionTitle(section, "Orientações para a proposta");
        var paragraph = section.AddParagraph();
        paragraph.AddText(
            "Informe preço unitário, marca ou modelo quando aplicável, frete, "
            + "prazo de entrega, condição de pagamento e validade da proposta.");
    }

    private static void AddOptionalDetails(
        Section section,
        QuotationRequestReportDataDto data)
    {
        if (string.IsNullOrWhiteSpace(data.Instructions))
        {
            return;
        }

        AddSectionTitle(section, "Observações");
        section.AddParagraph(data.Instructions);
    }

    private static void AddSectionTitle(Section section, string text) =>
        section.AddParagraph(text).Style = StyleNames.Heading1;

    private static string FormatDate(DateOnly value) =>
        value.ToString("dd/MM/yyyy", OperationalPdfTheme.BrazilianCulture);
}
