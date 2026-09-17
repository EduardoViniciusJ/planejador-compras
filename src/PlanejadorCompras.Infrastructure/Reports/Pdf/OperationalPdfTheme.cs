using System.Globalization;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using PlanejadorCompras.Infrastructure.Reports;

namespace PlanejadorCompras.Infrastructure.Reports.Pdf;

internal static class OperationalPdfTheme
{
    internal static readonly CultureInfo BrazilianCulture =
        CultureInfo.GetCultureInfo("pt-BR");

    internal static readonly Color TextColor = ToColor(ReportDesignSystem.Text);
    internal static readonly Color MutedColor = ToColor(ReportDesignSystem.Muted);
    internal static readonly Color BorderColor = ToColor(ReportDesignSystem.Border);
    internal static readonly Color HeaderBackground = ToColor(ReportDesignSystem.Surface);

    internal static Document CreateDocument(string title, string subject)
    {
        var document = new Document();
        document.Info.Title = title;
        document.Info.Subject = subject;

        var normal = document.Styles[StyleNames.Normal]!;
        normal.Font.Name = EmbeddedPdfFontResolver.FamilyName;
        normal.Font.Size = Unit.FromPoint(ReportDesignSystem.BodyFontSize);
        normal.Font.Color = TextColor;
        normal.ParagraphFormat.SpaceAfter = Unit.FromPoint(2);

        var heading = document.Styles[StyleNames.Heading1]!;
        heading.Font.Name = EmbeddedPdfFontResolver.FamilyName;
        heading.Font.Size = Unit.FromPoint(ReportDesignSystem.SectionFontSize);
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
        footer.Format.Font.Name = ReportDesignSystem.FontFamily;
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

    private static Color ToColor(ReportRgbColor color) =>
        Color.FromRgb(color.Red, color.Green, color.Blue);
}
