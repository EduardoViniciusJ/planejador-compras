using ClosedXML.Excel;

namespace PlanejadorCompras.Infrastructure.Reports.Excel;

internal static class ClosedXmlReportStyles
{
    private const int ExcelCellTextLimit = 32_767;
    private const string CurrencyFormat = "[$R$-pt-BR] #,##0.00";
    private const string DateTimeFormat = "dd/mm/yyyy hh:mm";
    private const string HeaderColor = "#1F1F1F";
    private const string SectionColor = "#F2F2F2";
    private const string BestPriceColor = "#E2F0D9";
    private const string BestPriceFontColor = "#006100";
    private const string MissingPriceColor = "#FFF2CC";
    private const string MissingPriceFontColor = "#9C6500";
    private const string BorderColor = "#D2D2D2";

    internal const string QuantityFormat = "0.###";

    internal static void ApplyWorksheetDefaults(IXLWorksheet worksheet)
    {
        worksheet.ShowGridLines = false;
        worksheet.Style.Font.FontName = "Aptos";
        worksheet.Style.Font.FontSize = 10;
        worksheet.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        worksheet.RowHeight = 20;
    }

    internal static void ApplyTitleStyle(IXLRange range)
    {
        range.Style.Fill.BackgroundColor = XLColor.FromHtml(HeaderColor);
        range.Style.Font.FontColor = XLColor.White;
        range.Style.Font.FontSize = 18;
        range.Style.Font.Bold = true;
        range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        range.Worksheet.Row(range.RangeAddress.FirstAddress.RowNumber).Height = 32;
    }

    internal static void ApplySectionStyle(IXLRange range)
    {
        range.Style.Fill.BackgroundColor = XLColor.FromHtml(SectionColor);
        range.Style.Font.FontColor = XLColor.FromHtml(HeaderColor);
        range.Style.Font.Bold = true;
    }

    internal static void ApplyHeaderStyle(IXLRange range)
    {
        range.Style.Fill.BackgroundColor = XLColor.FromHtml(HeaderColor);
        range.Style.Font.FontColor = XLColor.White;
        range.Style.Font.Bold = true;
        range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        range.Style.Alignment.WrapText = true;
        range.Worksheet.Row(range.RangeAddress.FirstAddress.RowNumber).Height = 34;
    }

    internal static void ApplyBestPriceStyle(IXLCell cell)
    {
        cell.Style.Fill.BackgroundColor = XLColor.FromHtml(BestPriceColor);
        cell.Style.Font.FontColor = XLColor.FromHtml(BestPriceFontColor);
        cell.Style.Font.Bold = true;
    }

    internal static void ApplyBestPriceStyle(IXLRange range)
    {
        range.Style.Fill.BackgroundColor = XLColor.FromHtml(BestPriceColor);
        range.Style.Font.FontColor = XLColor.FromHtml(BestPriceFontColor);
        range.Style.Font.Bold = true;
    }

    internal static void ApplyMissingStyle(IXLCell cell)
    {
        cell.Style.Fill.BackgroundColor = XLColor.FromHtml(MissingPriceColor);
        cell.Style.Font.FontColor = XLColor.FromHtml(MissingPriceFontColor);
    }

    internal static void ApplyMissingStyle(IXLRange range)
    {
        range.Style.Fill.BackgroundColor = XLColor.FromHtml(MissingPriceColor);
        range.Style.Font.FontColor = XLColor.FromHtml(MissingPriceFontColor);
    }

    internal static void ApplyRangeBorders(IXLRange range)
    {
        range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        range.Style.Border.OutsideBorderColor = XLColor.FromHtml(BorderColor);

        foreach (var row in range.Rows())
        {
            row.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            row.Style.Border.BottomBorderColor = XLColor.FromHtml(BorderColor);
        }
    }

    internal static void SetCurrency(IXLCell cell, decimal value)
    {
        cell.SetValue(value);
        cell.Style.NumberFormat.Format = CurrencyFormat;
    }

    internal static void ConfigurePrintLayout(
        IXLWorksheet worksheet,
        bool landscape)
    {
        worksheet.PageSetup.PageOrientation = landscape
            ? XLPageOrientation.Landscape
            : XLPageOrientation.Portrait;
        worksheet.PageSetup.PaperSize = XLPaperSize.A4Paper;
        worksheet.PageSetup.PagesWide = 1;
        worksheet.PageSetup.PagesTall = 0;
        worksheet.PageSetup.Margins.Top = 0.5;
        worksheet.PageSetup.Margins.Bottom = 0.5;
        worksheet.PageSetup.Margins.Left = 0.4;
        worksheet.PageSetup.Margins.Right = 0.4;
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
