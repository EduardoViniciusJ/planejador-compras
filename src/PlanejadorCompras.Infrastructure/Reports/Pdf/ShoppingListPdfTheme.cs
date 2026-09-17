using System.Globalization;
using MigraDoc.DocumentObjectModel;
using PlanejadorCompras.Infrastructure.Reports;

namespace PlanejadorCompras.Infrastructure.Reports.Pdf;

internal static class ShoppingListPdfTheme
{
    internal const int MaxSuppliersPerGroup = 4;
    internal const int HeaderDescriptionLimit = 180;
    internal const int TableTextLimit = 80;

    internal static readonly CultureInfo BrazilianCulture =
        CultureInfo.GetCultureInfo("pt-BR");
    internal static readonly Color DarkBlue = ToColor(ReportDesignSystem.Primary);
    internal static readonly Color TextColor = ToColor(ReportDesignSystem.Text);
    internal static readonly Color MutedColor = ToColor(ReportDesignSystem.Muted);
    internal static readonly Color BorderBlue = ToColor(ReportDesignSystem.Border);
    internal static readonly Color LightBlue = ToColor(ReportDesignSystem.Surface);
    internal static readonly Color BestPriceBackground = ToColor(ReportDesignSystem.BestPriceBackground);
    internal static readonly Color BestPriceForeground = ToColor(ReportDesignSystem.BestPriceForeground);
    internal static readonly Color MissingPriceBackground = ToColor(ReportDesignSystem.MissingPriceBackground);
    internal static readonly Color MissingPriceForeground = ToColor(ReportDesignSystem.MissingPriceForeground);

    internal static string FormatCurrency(decimal value) =>
        value.ToString("C2", BrazilianCulture);

    internal static string FormatOptionalCurrency(decimal? value) =>
        value.HasValue ? FormatCurrency(value.Value) : "Não disponível";

    internal static string LimitText(string value, int maximumLength) =>
        value.Length <= maximumLength
            ? value
            : $"{value[..(maximumLength - 3)]}...";

    private static Color ToColor(ReportRgbColor color) =>
        Color.FromRgb(color.Red, color.Green, color.Blue);
}
