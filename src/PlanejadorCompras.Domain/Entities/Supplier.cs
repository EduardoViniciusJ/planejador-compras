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
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new Supplier(
            Guid.NewGuid(),
            userId,
            name.Trim(),
            Normalize(cnpj),
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
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name.Trim();
        Cnpj = Normalize(cnpj);
        Address = address;
        Contact = contact;
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
