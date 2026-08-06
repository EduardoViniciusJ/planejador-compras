using PlanejadorCompras.Application.Features.Reports.Contracts;
using System.Globalization;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;

namespace PlanejadorCompras.Infrastructure.Reports.Pdf;

public sealed class PurchaseOrderPdfDocumentBuilder
{
    private static readonly CultureInfo BrazilianCulture =
        CultureInfo.GetCultureInfo("pt-BR");
    private static readonly Color TextColor = Color.FromRgb(24, 24, 27);
    private static readonly Color MutedColor = Color.FromRgb(82, 82, 91);
    private static readonly Color BorderColor = Color.FromRgb(212, 212, 216);
    private static readonly Color HeaderBackground = Color.FromRgb(244, 244, 245);

    public Document Build(PurchaseOrderReportDataDto data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var document = CreateDocument(data);
        var section = document.AddSection();
        ConfigureSection(section);
        AddFooter(section);
        AddTitle(section, data);
        AddGeneralInformation(section, data);
        AddItems(section, data);
        AddAdditionalInformation(section, data);

        return document;
    }

    private static Document CreateDocument(PurchaseOrderReportDataDto data)
    {
        var document = new Document();
        document.Info.Title = $"Pedido de compra {data.Code}";
        document.Info.Subject = "Pedido de compra";

        var normal = document.Styles[StyleNames.Normal]!;
        normal.Font.Name = EmbeddedPdfFontResolver.FamilyName;
        normal.Font.Size = Unit.FromPoint(9);
        normal.Font.Color = TextColor;
        normal.ParagraphFormat.SpaceAfter = Unit.FromPoint(2);

        var heading = document.Styles[StyleNames.Heading1]!;
        heading.Font.Name = EmbeddedPdfFontResolver.FamilyName;
        heading.Font.Size = Unit.FromPoint(12);
        heading.Font.Bold = true;
        heading.Font.Color = TextColor;
        heading.ParagraphFormat.SpaceBefore = Unit.FromPoint(10);
        heading.ParagraphFormat.SpaceAfter = Unit.FromPoint(5);
        heading.ParagraphFormat.KeepWithNext = true;

        return document;
    }

    private static void ConfigureSection(Section section)
    {
        section.PageSetup.PageFormat = PageFormat.A4;
        section.PageSetup.Orientation = Orientation.Portrait;
        section.PageSetup.LeftMargin = Unit.FromCentimeter(1.5);
        section.PageSetup.RightMargin = Unit.FromCentimeter(1.5);
        section.PageSetup.TopMargin = Unit.FromCentimeter(1.5);
        section.PageSetup.BottomMargin = Unit.FromCentimeter(1.5);
        section.PageSetup.FooterDistance = Unit.FromCentimeter(0.7);
    }

    private static void AddTitle(Section section, PurchaseOrderReportDataDto data)
    {
        var title = section.AddParagraph("Pedido de compra");
        title.Format.Font.Size = Unit.FromPoint(20);
        title.Format.Font.Bold = true;
        title.Format.SpaceAfter = Unit.FromPoint(3);

        var identification = section.AddParagraph();
        identification.Format.Font.Size = Unit.FromPoint(10);
        identification.Format.Font.Color = MutedColor;
        identification.Format.SpaceAfter = Unit.FromPoint(10);
        identification.AddFormattedText(data.Code, TextFormat.Bold);
        identification.AddText(
            $" | Emitido em {data.CreatedAtUtc.ToString("dd/MM/yyyy", BrazilianCulture)}"
            + $" | Situação: {data.Status}");
    }

    private static void AddGeneralInformation(
        Section section,
        PurchaseOrderReportDataDto data)
    {
        section.AddParagraph("Informações do pedido", StyleNames.Heading1);

        var table = section.AddTable();
        table.Borders.Width = Unit.FromPoint(0.5);
        table.Borders.Color = BorderColor;
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
            data.ExpectedDeliveryDate?.ToString("dd/MM/yyyy", BrazilianCulture)
                ?? "Não informada");
        AddLabelValue(
            thirdRow.Cells[2],
            thirdRow.Cells[3],
            "Condição de pagamento",
            data.PaymentTerms ?? "Não informada");
    }

    private static void AddItems(Section section, PurchaseOrderReportDataDto data)
    {
        section.AddParagraph("Itens", StyleNames.Heading1);

        var table = section.AddTable();
        table.Borders.Width = Unit.FromPoint(0.5);
        table.Borders.Color = BorderColor;
        table.AddColumn(Unit.FromCentimeter(7.2));
        table.AddColumn(Unit.FromCentimeter(2));
        table.AddColumn(Unit.FromCentimeter(1.6));
        table.AddColumn(Unit.FromCentimeter(3.4));
        table.AddColumn(Unit.FromCentimeter(3.8));

        var header = table.AddRow();
        header.HeadingFormat = true;
        header.Shading.Color = HeaderBackground;
        header.Format.Font.Bold = true;
        header.VerticalAlignment = VerticalAlignment.Center;
        AddCell(header.Cells[0], "Item", ParagraphAlignment.Left);
        AddCell(header.Cells[1], "Quantidade");
        AddCell(header.Cells[2], "Un.");
        AddCell(header.Cells[3], "Valor unitário", ParagraphAlignment.Right);
        AddCell(header.Cells[4], "Total", ParagraphAlignment.Right);

        foreach (var item in data.Items)
        {
            var row = table.AddRow();
            row.VerticalAlignment = VerticalAlignment.Center;
            row.TopPadding = Unit.FromPoint(4);
            row.BottomPadding = Unit.FromPoint(4);
            AddCell(row.Cells[0], LimitText(item.Name, 120), ParagraphAlignment.Left);
            AddCell(
                row.Cells[1],
                item.Quantity.ToString("0.###", BrazilianCulture));
            AddCell(row.Cells[2], LimitText(item.Unit, 20));
            AddCell(
                row.Cells[3],
                FormatCurrency(item.UnitPrice),
                ParagraphAlignment.Right);
            AddCell(
                row.Cells[4],
                FormatCurrency(item.TotalPrice),
                ParagraphAlignment.Right,
                bold: true);
        }

        var totalRow = table.AddRow();
        totalRow.Borders.Top.Width = Unit.FromPoint(1);
        totalRow.Format.Font.Bold = true;
        totalRow.Cells[0].MergeRight = 3;
        AddCell(totalRow.Cells[0], "Total do pedido", ParagraphAlignment.Right);
        AddCell(
            totalRow.Cells[4],
            FormatCurrency(data.TotalPrice),
            ParagraphAlignment.Right,
            bold: true);
    }

    private static void AddAdditionalInformation(
        Section section,
        PurchaseOrderReportDataDto data)
    {
        if (!string.IsNullOrWhiteSpace(data.DeliveryAddress))
        {
            AddTextSection(section, "Local de entrega", data.DeliveryAddress);
        }

        if (!string.IsNullOrWhiteSpace(data.Notes))
        {
            AddTextSection(section, "Observações", data.Notes);
        }
    }

    private static void AddTextSection(Section section, string title, string value)
    {
        section.AddParagraph(title, StyleNames.Heading1);
        var paragraph = section.AddParagraph(value);
        paragraph.Format.Borders.Width = Unit.FromPoint(0.5);
        paragraph.Format.Borders.Color = BorderColor;
        paragraph.Format.LeftIndent = Unit.FromPoint(6);
        paragraph.Format.RightIndent = Unit.FromPoint(6);
        paragraph.Format.SpaceBefore = Unit.FromPoint(2);
        paragraph.Format.SpaceAfter = Unit.FromPoint(4);
    }

    private static void AddFooter(Section section)
    {
        var footer = section.Footers.Primary.AddParagraph();
        footer.Format.Alignment = ParagraphAlignment.Right;
        footer.Format.Font.Size = Unit.FromPoint(8);
        footer.Format.Font.Color = MutedColor;
        footer.Format.Borders.Top.Width = Unit.FromPoint(0.5);
        footer.Format.Borders.Top.Color = BorderColor;
        footer.Format.SpaceBefore = Unit.FromPoint(3);
        footer.AddText("Página ");
        footer.AddPageField();
        footer.AddText(" de ");
        footer.AddNumPagesField();
    }

    private static void AddLabelValue(
        Cell labelCell,
        Cell valueCell,
        string label,
        string value)
    {
        labelCell.Shading.Color = HeaderBackground;
        labelCell.Format.Font.Bold = true;
        AddCell(labelCell, label, ParagraphAlignment.Left, bold: true);
        AddCell(valueCell, LimitText(value, 160), ParagraphAlignment.Left);
    }

    private static void AddCell(
        Cell cell,
        string text,
        ParagraphAlignment alignment = ParagraphAlignment.Center,
        bool bold = false)
    {
        cell.Format.Alignment = alignment;
        cell.VerticalAlignment = VerticalAlignment.Center;
        var paragraph = cell.AddParagraph();
        paragraph.Format.SpaceBefore = Unit.FromPoint(3);
        paragraph.Format.SpaceAfter = Unit.FromPoint(3);
        paragraph.AddFormattedText(text, bold ? TextFormat.Bold : TextFormat.NotBold);
    }

    private static string FormatCurrency(decimal value) =>
        value.ToString("C2", BrazilianCulture);

    private static string LimitText(string value, int maximumLength) =>
        value.Length <= maximumLength
            ? value
            : $"{value[..(maximumLength - 3)]}...";
}
