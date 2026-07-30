namespace PlanejadorCompras.Domain.Entities;

public sealed class PurchaseOrderItem
{
    private PurchaseOrderItem(
        Guid id,
        Guid purchaseOrderId,
        Guid sourceShoppingItemId,
        string name,
        decimal quantity,
        string unit,
        decimal unitPrice)
    {
        Id = id;
        PurchaseOrderId = purchaseOrderId;
        SourceShoppingItemId = sourceShoppingItemId;
        Name = name;
        Quantity = quantity;
        Unit = unit;
        UnitPrice = unitPrice;
    }

    public Guid Id { get; private set; }

    public Guid PurchaseOrderId { get; private set; }

    public Guid SourceShoppingItemId { get; private set; }

    public string Name { get; private set; } = null!;

    public decimal Quantity { get; private set; }

    public string Unit { get; private set; } = null!;

    public decimal UnitPrice { get; private set; }

    public decimal TotalPrice => Quantity * UnitPrice;

    internal static PurchaseOrderItem Create(
        Guid purchaseOrderId,
        PurchaseOrderItemSnapshot snapshot)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(purchaseOrderId, Guid.Empty);
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentOutOfRangeException.ThrowIfEqual(snapshot.SourceShoppingItemId, Guid.Empty);
        ArgumentException.ThrowIfNullOrWhiteSpace(snapshot.Name);
        ArgumentException.ThrowIfNullOrWhiteSpace(snapshot.Unit);

        if (snapshot.Quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(snapshot),
                "Purchase order item quantity must be greater than zero.");
        }

        if (snapshot.UnitPrice < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(snapshot),
                "Purchase order item unit price cannot be negative.");
        }

        return new PurchaseOrderItem(
            Guid.NewGuid(),
            purchaseOrderId,
            snapshot.SourceShoppingItemId,
            snapshot.Name.Trim(),
            snapshot.Quantity,
            snapshot.Unit.Trim(),
            snapshot.UnitPrice);
    }
}
