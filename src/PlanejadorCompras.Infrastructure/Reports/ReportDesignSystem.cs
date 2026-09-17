namespace PlanejadorCompras.Infrastructure.Reports;

internal readonly record struct ReportRgbColor(byte Red, byte Green, byte Blue)
{
    internal string Hex => $"#{Red:X2}{Green:X2}{Blue:X2}";
}

internal static class ReportDesignSystem
{
    // This font is embedded in PDF exports and is also written into the
    // workbook style so both formats use the same deterministic family.
    internal const string FontFamily = "Liberation Sans";

    internal const double BodyFontSize = 10;
    internal const double TableFontSize = 9.2;
    internal const double SmallFontSize = 8.5;
    internal const double FooterFontSize = 8;
    internal const double TitleFontSize = 18;
    internal const double SectionFontSize = 12;

    internal static readonly ReportRgbColor Primary = new(12, 12, 14);
    internal static readonly ReportRgbColor Text = new(24, 24, 27);
    internal static readonly ReportRgbColor Muted = new(82, 82, 91);
    internal static readonly ReportRgbColor Border = new(212, 212, 216);
    internal static readonly ReportRgbColor Surface = new(244, 244, 245);
    internal static readonly ReportRgbColor White = new(255, 255, 255);
    internal static readonly ReportRgbColor BestPriceBackground = new(226, 240, 217);
    internal static readonly ReportRgbColor BestPriceForeground = new(0, 97, 0);
    internal static readonly ReportRgbColor MissingPriceBackground = new(255, 242, 204);
    internal static readonly ReportRgbColor MissingPriceForeground = new(156, 101, 0);
}
