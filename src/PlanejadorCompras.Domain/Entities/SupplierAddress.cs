using PlanejadorCompras.Domain.Rules;
using PlanejadorCompras.Domain.Validation;

namespace PlanejadorCompras.Domain.Entities;

public sealed class SupplierAddress
{
    private SupplierAddress()
    {
    }

    private SupplierAddress(string? street, string? city, string? postalCode)
    {
        Street = DomainText.Optional(street, SupplierRules.StreetMaxLength, nameof(street));
        City = DomainText.Optional(city, SupplierRules.CityMaxLength, nameof(city));
        PostalCode = DomainText.Optional(
            postalCode,
            SupplierRules.PostalCodeLength,
            nameof(postalCode));
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
}
