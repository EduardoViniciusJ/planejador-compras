using PlanejadorCompras.Application.Features.Reports.Contracts;
using System.Globalization;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;

namespace PlanejadorCompras.Infrastructure.Reports.Pdf;

public sealed class QuotationRequestPdfDocumentBuilder
{
    private static readonly CultureInfo BrazilianCulture =
        CultureInfo.GetCultureInfo("pt-BR");
    private static readonly Color Ink = Color.FromRgb(24, 24, 27);
    private static readonly Color Muted = Color.FromRgb(82, 82, 91);
    private static readonly Color Border = Color.FromRgb(212, 212, 216);
    private static readonly Color HeaderBackground = Color.FromRgb(244, 244, 245);

    public Document Build(QuotationRequestReportDataDto data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var document = new Document();
        document.Info.Title = $"Solicitação de cotação {data.Code}";
        document.Info.Subject = "Solicitação de cotação";

        var normal = document.Styles[StyleNames.Normal]!;
        normal.Font.Name = EmbeddedPdfFontResolver.FamilyName;
        normal.Font.Size = Unit.FromPoint(9);
        normal.Font.Color = Ink;
        normal.ParagraphFormat.SpaceAfter = Unit.FromPoint(2);

        var heading = document.Styles[StyleNames.Heading1]!;
        heading.Font.Name = EmbeddedPdfFontResolver.FamilyName;
        heading.Font.Size = Unit.FromPoint(12);
        heading.Font.Bold = true;
        heading.Font.Color = Ink;
        heading.ParagraphFormat.SpaceBefore = Unit.FromPoint(10);
        heading.ParagraphFormat.SpaceAfter = Unit.FromPoint(5);
        heading.ParagraphFormat.KeepWithNext = true;

        var section = document.AddSection();
        section.PageSetup.PageFormat = PageFormat.A4;
        section.PageSetup.Orientation = Orientation.Portrait;
        section.PageSetup.LeftMargin = Unit.FromCentimeter(1.6);
        section.PageSetup.RightMargin = Unit.FromCentimeter(1.6);
        section.PageSetup.TopMargin = Unit.FromCentimeter(1.5);
        section.PageSetup.BottomMargin = Unit.FromCentimeter(1.5);
        section.PageSetup.FooterDistance = Unit.FromCentimeter(0.7);

        AddHeader(section, data);
        AddMetadata(section, data);
        AddItems(section, data.Items);
        AddRequestedInformation(section);
        AddOptionalDetails(section, data);
        AddFooter(section);

        return document;
    }

    private static void AddHeader(Section section, QuotationRequestReportDataDto data)
    {
        var title = section.AddParagraph("Solicitação de cotação");
        title.Format.Font.Size = Unit.FromPoint(20);
        title.Format.Font.Bold = true;
        title.Format.SpaceAfter = Unit.FromPoint(3);

        var identification = section.AddParagraph();
        identification.Format.Font.Size = Unit.FromPoint(10);
        identification.Format.Font.Color = Muted;
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
        table.Borders.Color = Border;
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
        row.Cells[0].Shading.Color = HeaderBackground;
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
        table.Borders.Color = Border;
        table.AddColumn(Unit.FromCentimeter(11.5));
        table.AddColumn(Unit.FromCentimeter(2.6));
        table.AddColumn(Unit.FromCentimeter(2.7));

        var header = table.AddRow();
        header.HeadingFormat = true;
        header.Shading.Color = HeaderBackground;
        header.Format.Font.Bold = true;
        AddCell(header.Cells[0], "Item", ParagraphAlignment.Left);
        AddCell(header.Cells[1], "Quantidade");
        AddCell(header.Cells[2], "Unidade");

        foreach (var item in items)
        {
            var row = table.AddRow();
            row.TopPadding = Unit.FromPoint(4);
            row.BottomPadding = Unit.FromPoint(4);
            AddCell(row.Cells[0], item.Name, ParagraphAlignment.Left);
            AddCell(row.Cells[1], item.Quantity.ToString("0.###", BrazilianCulture));
            AddCell(row.Cells[2], item.Unit);
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

    private static void AddFooter(Section section)
    {
        var footer = section.Footers.Primary.AddParagraph();
        footer.Format.Font.Size = Unit.FromPoint(7);
        footer.Format.Font.Color = Muted;
        footer.Format.Alignment = ParagraphAlignment.Right;
        footer.Format.Borders.Top.Width = Unit.FromPoint(0.5);
        footer.Format.Borders.Top.Color = Border;
        footer.AddText("Página ");
        footer.AddPageField();
        footer.AddText(" de ");
        footer.AddNumPagesField();
    }

    private static void AddSectionTitle(Section section, string text)
    {
        var paragraph = section.AddParagraph(text);
        paragraph.Style = StyleNames.Heading1;
    }

    private static void AddCell(
        Cell cell,
        string text,
        ParagraphAlignment alignment = ParagraphAlignment.Center)
    {
        cell.VerticalAlignment = VerticalAlignment.Center;
        cell.Format.Alignment = alignment;
        cell.AddParagraph(text);
    }

    private static string FormatDate(DateOnly value) =>
        value.ToString("dd/MM/yyyy", BrazilianCulture);
}
