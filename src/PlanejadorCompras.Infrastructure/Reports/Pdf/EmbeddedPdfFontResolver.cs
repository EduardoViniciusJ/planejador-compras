using PdfSharp.Fonts;

namespace PlanejadorCompras.Infrastructure.Reports.Pdf;

internal sealed class EmbeddedPdfFontResolver : IFontResolver
{
    public const string FamilyName = "Liberation Sans";

    private const string RegularFaceName = "LiberationSans#Regular";
    private const string BoldFaceName = "LiberationSans#Bold";
    private static readonly object ConfigurationLock = new();

    public static EmbeddedPdfFontResolver Instance { get; } = new();

    private EmbeddedPdfFontResolver()
    {
    }

    public FontResolverInfo ResolveTypeface(
        string familyName,
        bool isBold,
        bool isItalic)
    {
        return new FontResolverInfo(
            isBold ? BoldFaceName : RegularFaceName,
            mustSimulateBold: false,
            mustSimulateItalic: isItalic);
    }

    public byte[]? GetFont(string faceName)
    {
        return faceName switch
        {
            RegularFaceName => PdfEmbeddedAssets.LiberationSansRegular,
            BoldFaceName => PdfEmbeddedAssets.LiberationSansBold,
            _ => null
        };
    }

    public static void EnsureRegistered()
    {
        if (GlobalFontSettings.FontResolver is EmbeddedPdfFontResolver)
        {
            return;
        }

        lock (ConfigurationLock)
        {
            if (GlobalFontSettings.FontResolver is null)
            {
                GlobalFontSettings.FontResolver = Instance;
                return;
            }

            if (GlobalFontSettings.FontResolver is not EmbeddedPdfFontResolver)
            {
                throw new InvalidOperationException(
                    "PDFsharp already has a different global font resolver configured.");
            }
        }
    }
}
