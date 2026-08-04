namespace PlanejadorCompras.Domain.Entities;

public sealed class SupplierContact
{
    private SupplierContact()
    {
    }

    private SupplierContact(string? email, string? phone)
    {
        Email = Normalize(email)?.ToLowerInvariant();
        Phone = Normalize(phone);
    }

    public string? Email { get; private set; }

    public string? Phone { get; private set; }

    public static SupplierContact? Create(string? email, string? phone)
    {
        if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(phone))
        {
            return null;
        }

        return new SupplierContact(email, phone);
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
