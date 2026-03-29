namespace PlanejadorCompras.Domain.Entities;

public sealed class ShoppingList
{
    private ShoppingList(Guid id, Guid userId, string name, string? description, DateTime createdAt)
    {
        Id = id;
        UserId = userId;
        Name = name;
        Description = description;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public string Name { get; private set; }

    public string? Description { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public static ShoppingList Create(Guid userId, string name, string? description = null)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(userId, Guid.Empty);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new ShoppingList(
            Guid.NewGuid(),
            userId,
            name.Trim(),
            string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            DateTime.UtcNow);
    }
}
