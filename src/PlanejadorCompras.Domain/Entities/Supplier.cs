namespace PlanejadorCompras.Domain.Entities;

public sealed class Supplier
{
    private Supplier(Guid id, Guid userId, string name, DateTime createdAt)
    {
        Id = id;
        UserId = userId;
        Name = name;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public string Name { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public static Supplier Create(Guid userId, string name)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(userId, Guid.Empty);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new Supplier(Guid.NewGuid(), userId, name.Trim(), DateTime.UtcNow);
    }

    public void Update(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name.Trim();
    }
}
