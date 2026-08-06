using System.Globalization;
using MigraDoc.DocumentObjectModel;

namespace PlanejadorCompras.Infrastructure.Reports.Pdf;

internal static class ShoppingListPdfTheme
{
    internal const int MaxSuppliersPerGroup = 4;
    internal const int HeaderDescriptionLimit = 180;
    internal const int TableTextLimit = 80;

    internal static readonly CultureInfo BrazilianCulture =
        CultureInfo.GetCultureInfo("pt-BR");
    internal static readonly Color DarkBlue = Color.FromRgb(24, 24, 27);
    internal static readonly Color BorderBlue = Color.FromRgb(212, 212, 216);
    internal static readonly Color LightBlue = Color.FromRgb(244, 244, 245);
    internal static readonly Color BestPriceBackground = Color.FromRgb(226, 240, 217);
    internal static readonly Color BestPriceForeground = Color.FromRgb(0, 97, 0);
    internal static readonly Color MissingPriceBackground = Color.FromRgb(255, 242, 204);
    internal static readonly Color MissingPriceForeground = Color.FromRgb(156, 101, 0);

    internal static string FormatCurrency(decimal value) =>
        value.ToString("C2", BrazilianCulture);

    internal static string FormatOptionalCurrency(decimal? value) =>
        value.HasValue ? FormatCurrency(value.Value) : "Não disponível";

    internal static string LimitText(string value, int maximumLength) =>
        value.Length <= maximumLength
            ? value
            : $"{value[..(maximumLength - 3)]}...";
}
