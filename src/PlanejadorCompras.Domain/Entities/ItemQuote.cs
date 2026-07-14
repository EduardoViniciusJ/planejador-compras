namespace PlanejadorCompras.Domain.Entities;

public sealed class ItemQuote
{
    private ItemQuote(
        Guid id,
        Guid shoppingItemId,
        Guid supplierId,
        decimal unitPrice,
        DateTime createdAt)
    {
        Id = id;
        ShoppingItemId = shoppingItemId;
        SupplierId = supplierId;
        UnitPrice = unitPrice;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public Guid ShoppingItemId { get; private set; }

    public Guid SupplierId { get; private set; }

    public decimal UnitPrice { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public static ItemQuote Create(Guid shoppingItemId, Guid supplierId, decimal unitPrice)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(shoppingItemId, Guid.Empty);
        ArgumentOutOfRangeException.ThrowIfEqual(supplierId, Guid.Empty);

        if (unitPrice < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(unitPrice), "Unit price cannot be negative.");
        }

        return new ItemQuote(
            Guid.NewGuid(),
            shoppingItemId,
            supplierId,
            unitPrice,
            DateTime.UtcNow);
    }

    public void Update(Guid shoppingItemId, Guid supplierId, decimal unitPrice)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(shoppingItemId, Guid.Empty);
        ArgumentOutOfRangeException.ThrowIfEqual(supplierId, Guid.Empty);

        if (unitPrice < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(unitPrice), "Unit price cannot be negative.");
        }

        ShoppingItemId = shoppingItemId;
        SupplierId = supplierId;
        UnitPrice = unitPrice;
    }
}
