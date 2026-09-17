using ClosedXML.Excel;
using PlanejadorCompras.Infrastructure.Reports;

namespace PlanejadorCompras.Infrastructure.Reports.Excel;

internal static class ClosedXmlReportStyles
{
    private const int ExcelCellTextLimit = 32_767;
    private const string CurrencyFormat = "[$R$-pt-BR] #,##0.00";
    private const string DateTimeFormat = "dd/mm/yyyy hh:mm";
    internal const string QuantityFormat = "0.###";

    internal static void ApplyWorksheetDefaults(IXLWorksheet worksheet)
    {
        worksheet.ShowGridLines = false;
        worksheet.Style.Font.FontName = ReportDesignSystem.FontFamily;
        worksheet.Style.Font.FontSize = ReportDesignSystem.BodyFontSize;
        worksheet.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        worksheet.RowHeight = 22;
    }

    internal static void ApplyTitleStyle(IXLRange range)
    {
        range.Style.Fill.BackgroundColor = XLColor.FromHtml(ReportDesignSystem.Primary.Hex);
        range.Style.Font.FontColor = XLColor.White;
        range.Style.Font.FontSize = ReportDesignSystem.TitleFontSize;
        range.Style.Font.Bold = true;
        range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        range.Worksheet.Row(range.RangeAddress.FirstAddress.RowNumber).Height = 36;
    }

    internal static void ApplySectionStyle(IXLRange range)
    {
        range.Style.Fill.BackgroundColor = XLColor.FromHtml(ReportDesignSystem.Surface.Hex);
        range.Style.Font.FontColor = XLColor.FromHtml(ReportDesignSystem.Text.Hex);
        range.Style.Font.FontSize = ReportDesignSystem.SectionFontSize;
        range.Style.Font.Bold = true;
        range.Worksheet.Row(range.RangeAddress.FirstAddress.RowNumber).Height = 25;
    }

    internal static void ApplyHeaderStyle(IXLRange range)
    {
        range.Style.Fill.BackgroundColor = XLColor.FromHtml(ReportDesignSystem.Primary.Hex);
        range.Style.Font.FontColor = XLColor.White;
        range.Style.Font.FontSize = ReportDesignSystem.TableFontSize;
        range.Style.Font.Bold = true;
        range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        range.Style.Alignment.WrapText = true;
        range.Worksheet.Row(range.RangeAddress.FirstAddress.RowNumber).Height = 36;
    }

    internal static void ApplyBestPriceStyle(IXLCell cell)
    {
        cell.Style.Fill.BackgroundColor = XLColor.FromHtml(ReportDesignSystem.BestPriceBackground.Hex);
        cell.Style.Font.FontColor = XLColor.FromHtml(ReportDesignSystem.BestPriceForeground.Hex);
        cell.Style.Font.Bold = true;
    }

    internal static void ApplyBestPriceStyle(IXLRange range)
    {
        range.Style.Fill.BackgroundColor = XLColor.FromHtml(ReportDesignSystem.BestPriceBackground.Hex);
        range.Style.Font.FontColor = XLColor.FromHtml(ReportDesignSystem.BestPriceForeground.Hex);
        range.Style.Font.Bold = true;
    }

    internal static void ApplyMissingStyle(IXLCell cell)
    {
        cell.Style.Fill.BackgroundColor = XLColor.FromHtml(ReportDesignSystem.MissingPriceBackground.Hex);
        cell.Style.Font.FontColor = XLColor.FromHtml(ReportDesignSystem.MissingPriceForeground.Hex);
    }

    internal static void ApplyMissingStyle(IXLRange range)
    {
        range.Style.Fill.BackgroundColor = XLColor.FromHtml(ReportDesignSystem.MissingPriceBackground.Hex);
        range.Style.Font.FontColor = XLColor.FromHtml(ReportDesignSystem.MissingPriceForeground.Hex);
    }

    internal static void ApplyRangeBorders(IXLRange range)
    {
        range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        range.Style.Border.OutsideBorderColor = XLColor.FromHtml(ReportDesignSystem.Border.Hex);

        foreach (var row in range.Rows())
        {
            row.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            row.Style.Border.BottomBorderColor = XLColor.FromHtml(ReportDesignSystem.Border.Hex);
        }
    }

    internal static void SetCurrency(IXLCell cell, decimal value)
    {
        cell.SetValue(value);
        cell.Style.NumberFormat.Format = CurrencyFormat;
    }

    internal static void ConfigurePrintLayout(
        IXLWorksheet worksheet,
        bool landscape,
        bool fitToPageWidth,
        int printAreaLastRow,
        int printAreaLastColumn,
        int? repeatedHeaderRow = null)
    {
        worksheet.PageSetup.PageOrientation = landscape
            ? XLPageOrientation.Landscape
            : XLPageOrientation.Portrait;
        worksheet.PageSetup.PaperSize = XLPaperSize.A4Paper;
        worksheet.PageSetup.PagesWide = fitToPageWidth ? 1 : 0;
        worksheet.PageSetup.PagesTall = 0;
        worksheet.PageSetup.Margins.Top = 0.5;
        worksheet.PageSetup.Margins.Bottom = 0.5;
        worksheet.PageSetup.Margins.Left = 0.4;
        worksheet.PageSetup.Margins.Right = 0.4;
        worksheet.PageSetup.PrintAreas.Clear();
        worksheet.PageSetup.PrintAreas.Add(
            1,
            1,
            printAreaLastRow,
            printAreaLastColumn);

        if (repeatedHeaderRow.HasValue)
        {
            worksheet.PageSetup.SetRowsToRepeatAtTop(
                repeatedHeaderRow.Value,
                repeatedHeaderRow.Value);
        }
    }

    internal static void SetDateTime(IXLCell cell, DateTime value)
    {
        cell.SetValue(value);
        cell.Style.NumberFormat.Format = DateTimeFormat;
    }

    internal static void SetText(IXLCell cell, string? value)
    {
        var text = value ?? string.Empty;
        if (text.Length > ExcelCellTextLimit)
        {
            text = text[..ExcelCellTextLimit];
        }

        cell.SetValue(text);
    }
}
