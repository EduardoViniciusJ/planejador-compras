namespace PlanejadorCompras.Domain.Entities;

public sealed class SupplierAddress
{
    private SupplierAddress()
    {
    }

    private SupplierAddress(string? street, string? city, string? postalCode)
    {
        Street = Normalize(street);
        City = Normalize(city);
        PostalCode = Normalize(postalCode);
    }

    public string? Street { get; private set; }

    public string? City { get; private set; }

    public string? PostalCode { get; private set; }

    public static SupplierAddress? Create(string? street, string? city, string? postalCode)
    {
        if (string.IsNullOrWhiteSpace(street)
            && string.IsNullOrWhiteSpace(city)
            && string.IsNullOrWhiteSpace(postalCode))
        {
            return null;
        }

        return new SupplierAddress(street, city, postalCode);
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
