using System.Reflection;

namespace PlanejadorCompras.Infrastructure.Reports.Pdf;

internal static class PdfEmbeddedAssets
{
    private const string ResourcePrefix =
        "PlanejadorCompras.Infrastructure.Reports.Pdf.Assets";

    private static readonly Lazy<byte[]> RegularFont =
        new(() => LoadResource($"{ResourcePrefix}.Fonts.LiberationSans-Regular.ttf"));
    private static readonly Lazy<byte[]> BoldFont =
        new(() => LoadResource($"{ResourcePrefix}.Fonts.LiberationSans-Bold.ttf"));

    public static byte[] LiberationSansRegular => RegularFont.Value;
    public static byte[] LiberationSansBold => BoldFont.Value;

    private static byte[] LoadResource(string resourceName)
    {
        var assembly = typeof(PdfEmbeddedAssets).Assembly;

        using var resourceStream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException(
                $"Embedded PDF resource '{resourceName}' was not found.");
        using var memoryStream = new MemoryStream();
        resourceStream.CopyTo(memoryStream);

        return memoryStream.ToArray();
    }
}
