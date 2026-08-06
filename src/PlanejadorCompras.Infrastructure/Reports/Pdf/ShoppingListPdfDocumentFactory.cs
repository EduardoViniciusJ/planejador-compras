using MigraDoc.DocumentObjectModel;
using PlanejadorCompras.Application.Features.Reports.Contracts;

namespace PlanejadorCompras.Infrastructure.Reports.Pdf;

internal static class ShoppingListPdfDocumentFactory
{
    internal static Document Create(ShoppingListReportDataDto reportData)
    {
        var document = new Document();
        document.Info.Title =
            $"Equalização - {ShoppingListPdfTheme.LimitText(reportData.Name, ShoppingListPdfTheme.TableTextLimit)}";
        document.Info.Subject = "Relatório de equalização de preços";

        var normalStyle = document.Styles[StyleNames.Normal]!;
        normalStyle.Font.Name = EmbeddedPdfFontResolver.FamilyName;
        normalStyle.Font.Size = Unit.FromPoint(8.5);
        normalStyle.ParagraphFormat.SpaceAfter = Unit.FromPoint(2);

        var headingStyle = document.Styles[StyleNames.Heading1]!;
        headingStyle.Font.Name = EmbeddedPdfFontResolver.FamilyName;
        headingStyle.Font.Size = Unit.FromPoint(13);
        headingStyle.Font.Bold = true;
        headingStyle.Font.Color = ShoppingListPdfTheme.DarkBlue;
        headingStyle.ParagraphFormat.SpaceBefore = Unit.FromPoint(7);
        headingStyle.ParagraphFormat.SpaceAfter = Unit.FromPoint(5);
        headingStyle.ParagraphFormat.KeepWithNext = true;

        return document;
    }

    internal static Section AddSection(Document document)
    {
        var section = document.AddSection();
        section.PageSetup.PageFormat = PageFormat.A4;
        section.PageSetup.Orientation = Orientation.Landscape;
        section.PageSetup.LeftMargin = Unit.FromCentimeter(1.2);
        section.PageSetup.RightMargin = Unit.FromCentimeter(1.2);
        section.PageSetup.TopMargin = Unit.FromCentimeter(3.1);
        section.PageSetup.BottomMargin = Unit.FromCentimeter(1.6);
        section.PageSetup.HeaderDistance = Unit.FromCentimeter(0.6);
        section.PageSetup.FooterDistance = Unit.FromCentimeter(0.7);
        section.PageSetup.DifferentFirstPageHeaderFooter = false;
        section.PageSetup.OddAndEvenPagesHeaderFooter = false;
        return section;
    }
}
