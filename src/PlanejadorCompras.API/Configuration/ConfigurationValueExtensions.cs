namespace PlanejadorCompras.API.Configuration;

internal static class ConfigurationValueExtensions
{
    internal static string GetRequiredValue(
        this IConfiguration configuration,
        string key)
    {
        var value = configuration[key];

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Missing configuration '{key}'.");
        }

        return value;
    }

    internal static string GetRequiredSecret(
        this IConfiguration configuration,
        string key,
        int minimumByteLength)
    {
        var value = configuration.GetRequiredValue(key);
        var byteLength = System.Text.Encoding.UTF8.GetByteCount(value);

        if (byteLength < minimumByteLength)
        {
            throw new InvalidOperationException(
                $"Configuration '{key}' must contain at least "
                + $"{minimumByteLength} UTF-8 bytes.");
        }

        return value;
    }
}
