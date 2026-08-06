using System.Globalization;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;

namespace PlanejadorCompras.Infrastructure.Reports.Pdf;

internal static class OperationalPdfTheme
{
    internal static readonly CultureInfo BrazilianCulture =
        CultureInfo.GetCultureInfo("pt-BR");

    internal static readonly Color TextColor = Color.FromRgb(24, 24, 27);
    internal static readonly Color MutedColor = Color.FromRgb(82, 82, 91);
    internal static readonly Color BorderColor = Color.FromRgb(212, 212, 216);
    internal static readonly Color HeaderBackground = Color.FromRgb(244, 244, 245);

    internal static Document CreateDocument(string title, string subject)
    {
        var document = new Document();
        document.Info.Title = title;
        document.Info.Subject = subject;

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

    internal static Section AddPortraitSection(
        Document document,
        double horizontalMarginCentimeters)
    {
        var section = document.AddSection();
        section.PageSetup.PageFormat = PageFormat.A4;
        section.PageSetup.Orientation = Orientation.Portrait;
        section.PageSetup.LeftMargin = Unit.FromCentimeter(horizontalMarginCentimeters);
        section.PageSetup.RightMargin = Unit.FromCentimeter(horizontalMarginCentimeters);
        section.PageSetup.TopMargin = Unit.FromCentimeter(1.5);
        section.PageSetup.BottomMargin = Unit.FromCentimeter(1.5);
        section.PageSetup.FooterDistance = Unit.FromCentimeter(0.7);
        return section;
    }

    internal static void AddFooter(Section section, double fontSize)
    {
        var footer = section.Footers.Primary.AddParagraph();
        footer.Format.Alignment = ParagraphAlignment.Right;
        footer.Format.Font.Size = Unit.FromPoint(fontSize);
        footer.Format.Font.Color = MutedColor;
        footer.Format.Borders.Top.Width = Unit.FromPoint(0.5);
        footer.Format.Borders.Top.Color = BorderColor;
        footer.Format.SpaceBefore = Unit.FromPoint(3);
        footer.AddText("Página ");
        footer.AddPageField();
        footer.AddText(" de ");
        footer.AddNumPagesField();
    }

    internal static void AddCell(
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

    internal static string FormatCurrency(decimal value) =>
        value.ToString("C2", BrazilianCulture);

    internal static string LimitText(string value, int maximumLength) =>
        value.Length <= maximumLength
            ? value
            : $"{value[..(maximumLength - 3)]}...";
}
