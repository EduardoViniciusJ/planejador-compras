namespace PlanejadorCompras.Domain.Entities;

public sealed class ShoppingItem
{
    private ShoppingItem(Guid id, Guid shoppingListId, string name, decimal quantity, string unit, DateTime createdAt)
    {
        Id = id;
        ShoppingListId = shoppingListId;
        Name = name;
        Quantity = quantity;
        Unit = unit;
        CreatedAt = createdAt;
    }


    public Guid Id { get; private set; }

    public Guid ShoppingListId { get; private set; }

    public string Name { get; private set; }

    public decimal Quantity { get; private set; }

    public string Unit { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public static ShoppingItem Create(Guid shoppingListId, string name, decimal quantity, string unit)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(shoppingListId, Guid.Empty);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(unit);

        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");
        }

        return new ShoppingItem(
            Guid.NewGuid(),
            shoppingListId,
            name.Trim(),
            quantity,
            unit.Trim(),
            DateTime.UtcNow);
    }

    public void Update(Guid shoppingListId, string name, decimal quantity, string unit)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(shoppingListId, Guid.Empty);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(unit);

        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");
        }

        ShoppingListId = shoppingListId;
        Name = name.Trim();
        Quantity = quantity;
        Unit = unit.Trim();
    }
}
