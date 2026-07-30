using System.Globalization;
using System.Text;

namespace PlanejadorCompras.Infrastructure.Reports;

internal static class ReportFileNameBuilder
{
    private const int MaximumBaseNameLength = 80;

    public static string BuildEqualizationFileName(
        string shoppingListName,
        string extension)
    {
        return BuildFileName(shoppingListName, "equalizacao", extension);
    }

    public static string BuildPurchaseOrderFileName(
        string purchaseOrderCode,
        string extension)
    {
        return BuildFileName(
            $"pedido-compra-{purchaseOrderCode}",
            "pedido-compra",
            extension);
    }

    private static string BuildFileName(
        string? value,
        string fallbackBaseName,
        string extension)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(extension);

        var safeExtension = extension.TrimStart('.');

        if (safeExtension.Any(character => !char.IsAsciiLetterOrDigit(character)))
        {
            throw new ArgumentException("File extension contains invalid characters.", nameof(extension));
        }

        var safeBaseName = BuildSafeBaseName(value);

        if (string.IsNullOrEmpty(safeBaseName))
        {
            safeBaseName = fallbackBaseName;
        }

        return $"{safeBaseName}.{safeExtension.ToLowerInvariant()}";
    }

    private static string BuildSafeBaseName(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var normalizedValue = value.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalizedValue.Length);
        var separatorPending = false;

        foreach (var character in normalizedValue)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) ==
                UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            if (char.IsAsciiLetterOrDigit(character))
            {
                if (separatorPending && builder.Length > 0)
                {
                    builder.Append('-');
                }

                builder.Append(char.ToLowerInvariant(character));
                separatorPending = false;
                continue;
            }

            separatorPending = builder.Length > 0;
        }

        var safeBaseName = builder
            .ToString()
            .TrimEnd('-');

        if (safeBaseName.Length <= MaximumBaseNameLength)
        {
            return safeBaseName;
        }

        return safeBaseName[..MaximumBaseNameLength].TrimEnd('-');
    }
}
