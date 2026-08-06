using PlanejadorCompras.Domain.Rules;
using PlanejadorCompras.Domain.Validation;

namespace PlanejadorCompras.Domain.Entities;

public sealed class ShoppingItem
{
    private ShoppingItem(
        Guid id,
        Guid shoppingListId,
        string name,
        decimal quantity,
        string unit,
        DateTime createdAt)
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

    public static ShoppingItem Create(
        Guid shoppingListId,
        string name,
        decimal quantity,
        string unit)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(shoppingListId, Guid.Empty);
        EnsureValidQuantity(quantity);

        return new ShoppingItem(
            Guid.NewGuid(),
            shoppingListId,
            DomainText.Required(name, ShoppingItemRules.NameMaxLength, nameof(name)),
            quantity,
            DomainText.Required(unit, ShoppingItemRules.UnitMaxLength, nameof(unit)),
            DateTime.UtcNow);
    }

    public void Update(
        Guid shoppingListId,
        string name,
        decimal quantity,
        string unit)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(shoppingListId, Guid.Empty);
        EnsureValidQuantity(quantity);

        ShoppingListId = shoppingListId;
        Name = DomainText.Required(name, ShoppingItemRules.NameMaxLength, nameof(name));
        Quantity = quantity;
        Unit = DomainText.Required(unit, ShoppingItemRules.UnitMaxLength, nameof(unit));
    }

    private static void EnsureValidQuantity(decimal quantity)
    {
        if (quantity < ShoppingItemRules.MinimumQuantity
            || quantity > ShoppingItemRules.MaximumQuantity)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                $"Quantity must be between {ShoppingItemRules.MinimumQuantity} "
                + $"and {ShoppingItemRules.MaximumQuantity}.");
        }
    }
}
