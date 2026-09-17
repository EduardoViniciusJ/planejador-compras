using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using PlanejadorCompras.Application.Features.Reports.Contracts;
using PlanejadorCompras.Infrastructure.Reports;

namespace PlanejadorCompras.Infrastructure.Reports.Pdf;

internal static class ShoppingListPdfSummaryBuilder
{
    internal static void Add(
        Section section,
        ShoppingListReportDataDto reportData)
    {
        section.AddParagraph("Resumo da decisão", StyleNames.Heading1);

        var table = section.AddTable();
        table.Format.Font.Name = ReportDesignSystem.FontFamily;
        table.Format.Font.Size = Unit.FromPoint(ReportDesignSystem.TableFontSize);
        table.Borders.Width = Unit.FromPoint(0.4);
        table.Borders.Color = ShoppingListPdfTheme.BorderBlue;

        for (var column = 0; column < 4; column++)
        {
            table.AddColumn(Unit.FromCentimeter(6.68));
        }

        var row = table.AddRow();
        row.TopPadding = Unit.FromPoint(5);
        row.BottomPadding = Unit.FromPoint(5);
        AddCell(
            row.Cells[0],
            "Menores preços por item",
            ShoppingListPdfTheme.FormatCurrency(reportData.Summary.BestChoiceTotal));
        AddCell(
            row.Cells[1],
            "Melhor fornecedor completo",
            ShoppingListPdfTheme.LimitText(
                reportData.Summary.BestCompleteSupplierName ?? "Não disponível",
                42));
        AddCell(
            row.Cells[2],
            "Total do fornecedor",
            ShoppingListPdfTheme.FormatOptionalCurrency(
                reportData.Summary.BestCompleteSupplierTotal));
        AddCell(
            row.Cells[3],
            "Economia estimada",
            ShoppingListPdfTheme.FormatOptionalCurrency(
                reportData.Summary.PotentialSavings));
    }

    internal static void AddPriceMapTitle(
        Section section,
        int groupIndex,
        int groupCount,
        int supplierCount)
    {
        section.AddParagraph("Mapa comparativo", StyleNames.Heading1);

        if (groupCount <= 1)
        {
            return;
        }

        var firstSupplier =
            (groupIndex * ShoppingListPdfTheme.MaxSuppliersPerGroup) + 1;
        var lastSupplier = Math.Min(
            firstSupplier + ShoppingListPdfTheme.MaxSuppliersPerGroup - 1,
            supplierCount);
        var context = section.AddParagraph(
            $"Fornecedores {firstSupplier} a {lastSupplier} de {supplierCount}");
        context.Format.Font.Size = Unit.FromPoint(ReportDesignSystem.SmallFontSize);
        context.Format.Font.Color = ShoppingListPdfTheme.MutedColor;
        context.Format.SpaceAfter = Unit.FromPoint(4);
        context.Format.KeepWithNext = true;
    }

    private static void AddCell(Cell cell, string label, string value)
    {
        cell.VerticalAlignment = VerticalAlignment.Center;
        cell.Format.Alignment = ParagraphAlignment.Center;
        cell.Shading.Color = ShoppingListPdfTheme.LightBlue;

        var labelParagraph = cell.AddParagraph();
        labelParagraph.Format.Font.Name = ReportDesignSystem.FontFamily;
        labelParagraph.Format.Font.Size = Unit.FromPoint(ReportDesignSystem.SmallFontSize);
        labelParagraph.Format.Font.Color = ShoppingListPdfTheme.MutedColor;
        labelParagraph.AddText(label);

        var valueParagraph = cell.AddParagraph();
        valueParagraph.Format.Font.Name = ReportDesignSystem.FontFamily;
        valueParagraph.Format.Font.Size = Unit.FromPoint(ReportDesignSystem.BodyFontSize);
        valueParagraph.Format.Font.Bold = true;
        valueParagraph.Format.Font.Color = ShoppingListPdfTheme.DarkBlue;
        valueParagraph.AddText(value);
    }
}
