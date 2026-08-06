using PlanejadorCompras.Domain.Rules;
using PlanejadorCompras.Domain.Validation;

namespace PlanejadorCompras.Domain.Entities;

public sealed class SupplierContact
{
    private SupplierContact()
    {
    }

    private SupplierContact(string? email, string? phone)
    {
        Email = DomainText.Optional(email, SupplierRules.EmailMaxLength, nameof(email))?
            .ToLowerInvariant();
        Phone = DomainText.Optional(phone, SupplierRules.PhoneMaxLength, nameof(phone));
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
}
