using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using PlanejadorCompras.Application.Features.Reports.Contracts;

namespace PlanejadorCompras.Infrastructure.Reports.Pdf;

internal static class ShoppingListPdfPageChrome
{
    internal static void AddHeader(
        Section section,
        ShoppingListReportDataDto reportData)
    {
        var headerTable = section.Headers.Primary.AddTable();
        headerTable.AddColumn(Unit.FromCentimeter(26.9));
        headerTable.Borders.Bottom.Width = Unit.FromPoint(0.8);
        headerTable.Borders.Bottom.Color = ShoppingListPdfTheme.BorderBlue;

        var row = headerTable.AddRow();
        row.VerticalAlignment = VerticalAlignment.Center;

        var listParagraph = row.Cells[0].AddParagraph();
        listParagraph.Format.SpaceAfter = Unit.FromPoint(1);
        listParagraph.AddFormattedText("Lista: ", TextFormat.Bold);
        listParagraph.AddText(
            ShoppingListPdfTheme.LimitText(
                reportData.Name,
                ShoppingListPdfTheme.TableTextLimit));

        if (!string.IsNullOrWhiteSpace(reportData.Description))
        {
            var descriptionParagraph = row.Cells[0].AddParagraph();
            descriptionParagraph.Format.Font.Size = Unit.FromPoint(7.2);
            descriptionParagraph.Format.Font.Color = Colors.DimGray;
            descriptionParagraph.AddText(
                ShoppingListPdfTheme.LimitText(
                    reportData.Description,
                    ShoppingListPdfTheme.HeaderDescriptionLimit));
        }

        var generatedParagraph = row.Cells[0].AddParagraph();
        generatedParagraph.Format.Font.Size = Unit.FromPoint(7.2);
        generatedParagraph.Format.Font.Color = Colors.DimGray;
        generatedParagraph.AddText(
            $"Gerado em {reportData.GeneratedAt.ToString("dd/MM/yyyy HH:mm", ShoppingListPdfTheme.BrazilianCulture)}");
    }

    internal static void AddFooter(Section section)
    {
        var footer = section.Footers.Primary.AddParagraph();
        footer.Format.Font.Size = Unit.FromPoint(7);
        footer.Format.Font.Color = Colors.DimGray;
        footer.Format.Alignment = ParagraphAlignment.Right;
        footer.Format.Borders.Top.Width = Unit.FromPoint(0.5);
        footer.Format.Borders.Top.Color = ShoppingListPdfTheme.BorderBlue;
        footer.Format.SpaceBefore = Unit.FromPoint(3);
        footer.AddText("Página ");
        footer.AddPageField();
        footer.AddText(" de ");
        footer.AddNumPagesField();
    }

    internal static void AddIntroduction(
        Section section,
        ShoppingListReportDataDto reportData)
    {
        var title = section.AddParagraph("Equalização de preços");
        title.Format.Font.Size = Unit.FromPoint(20);
        title.Format.Font.Bold = true;
        title.Format.Font.Color = ShoppingListPdfTheme.DarkBlue;
        title.Format.SpaceAfter = Unit.FromPoint(3);
        title.Format.KeepWithNext = true;

        var subtitle = section.AddParagraph(
            "Comparativo consolidado dos preços informados pelos fornecedores.");
        subtitle.Format.Font.Size = Unit.FromPoint(8);
        subtitle.Format.Font.Color = Colors.DimGray;
        subtitle.Format.SpaceAfter = Unit.FromPoint(7);
        subtitle.Format.KeepWithNext = true;

        if (reportData.Items.Count == 0)
        {
            AddEmptyListWarning(section);
        }
    }

    private static void AddEmptyListWarning(Section section)
    {
        var warning = section.AddParagraph(
            "Esta lista ainda não possui itens cadastrados.");
        warning.Format.Shading.Color = ShoppingListPdfTheme.MissingPriceBackground;
        warning.Format.Font.Color = ShoppingListPdfTheme.MissingPriceForeground;
        warning.Format.LeftIndent = Unit.FromPoint(5);
        warning.Format.RightIndent = Unit.FromPoint(5);
        warning.Format.SpaceBefore = Unit.FromPoint(3);
        warning.Format.SpaceAfter = Unit.FromPoint(7);
    }
}
