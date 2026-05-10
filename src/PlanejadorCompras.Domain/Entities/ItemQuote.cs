namespace PlanejadorCompras.Domain.Entities;

public sealed class ItemQuote
{
    private ItemQuote(
        Guid id,
        Guid shoppingItemId,
        string supplierName,
        decimal unitPrice,
        DateTime createdAt)
    {
        Id = id;
        ShoppingItemId = shoppingItemId;
        SupplierName = supplierName;
        UnitPrice = unitPrice;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public Guid ShoppingItemId { get; private set; }

    public string SupplierName { get; private set; }

    public decimal UnitPrice { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public static ItemQuote Create(Guid shoppingItemId, string supplierName, decimal unitPrice)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(shoppingItemId, Guid.Empty);
        ArgumentException.ThrowIfNullOrWhiteSpace(supplierName);

        if (unitPrice < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(unitPrice), "Unit price cannot be negative.");
        }

        return new ItemQuote(
            Guid.NewGuid(),
            shoppingItemId,
            supplierName.Trim(),
            unitPrice,
            DateTime.UtcNow);
    }

    public void Update(Guid shoppingItemId, string supplierName, decimal unitPrice)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(shoppingItemId, Guid.Empty);
        ArgumentException.ThrowIfNullOrWhiteSpace(supplierName);

        if (unitPrice < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(unitPrice), "Unit price cannot be negative.");
        }

        ShoppingItemId = shoppingItemId;
        SupplierName = supplierName.Trim();
        UnitPrice = unitPrice;
    }
}
