using PlanejadorCompras.Domain.Rules;
using PlanejadorCompras.Domain.Validation;

namespace PlanejadorCompras.Domain.Entities;

public sealed class Supplier
{
    private Supplier()
    {
    }

    private Supplier(
        Guid id,
        Guid userId,
        string name,
        string? cnpj,
        SupplierAddress? address,
        SupplierContact? contact,
        DateTime createdAt)
    {
        Id = id;
        UserId = userId;
        Name = name;
        Cnpj = cnpj;
        Address = address;
        Contact = contact;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public string Name { get; private set; } = null!;

    public string? Cnpj { get; private set; }

    public SupplierAddress? Address { get; private set; }

    public SupplierContact? Contact { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public static Supplier Create(
        Guid userId,
        string name,
        string? cnpj = null,
        SupplierAddress? address = null,
        SupplierContact? contact = null)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(userId, Guid.Empty);
        return new Supplier(
            Guid.NewGuid(),
            userId,
            DomainText.Required(name, SupplierRules.NameMaxLength, nameof(name)),
            DomainText.Optional(cnpj, SupplierRules.CnpjLength, nameof(cnpj)),
            address,
            contact,
            DateTime.UtcNow);
    }

    public void Update(
        string name,
        string? cnpj = null,
        SupplierAddress? address = null,
        SupplierContact? contact = null)
    {
        Name = DomainText.Required(name, SupplierRules.NameMaxLength, nameof(name));
        Cnpj = DomainText.Optional(cnpj, SupplierRules.CnpjLength, nameof(cnpj));
        Address = address;
        Contact = contact;
    }
}
