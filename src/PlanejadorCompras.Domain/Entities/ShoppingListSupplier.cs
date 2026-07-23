namespace PlanejadorCompras.Domain.Entities;

public sealed class ShoppingListSupplier
{
    private ShoppingListSupplier(Guid shoppingListId, Guid supplierId, DateTime createdAt)
    {
        ShoppingListId = shoppingListId;
        SupplierId = supplierId;
        CreatedAt = createdAt;
    }

    public Guid ShoppingListId { get; private set; }

    public Guid SupplierId { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public static ShoppingListSupplier Create(Guid shoppingListId, Guid supplierId)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(shoppingListId, Guid.Empty);
        ArgumentOutOfRangeException.ThrowIfEqual(supplierId, Guid.Empty);

        return new ShoppingListSupplier(shoppingListId, supplierId, DateTime.UtcNow);
    }
}
