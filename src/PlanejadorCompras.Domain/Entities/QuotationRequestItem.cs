namespace PlanejadorCompras.Domain.Entities;

public sealed class QuotationRequestItem
{
    private QuotationRequestItem()
    {
    }

    private QuotationRequestItem(
        Guid id,
        Guid quotationRequestId,
        Guid sourceShoppingItemId,
        int position,
        string name,
        decimal quantity,
        string unit)
    {
        Id = id;
        QuotationRequestId = quotationRequestId;
        SourceShoppingItemId = sourceShoppingItemId;
        Position = position;
        Name = name;
        Quantity = quantity;
        Unit = unit;
    }

    public Guid Id { get; private set; }
    public Guid QuotationRequestId { get; private set; }
    public Guid? SourceShoppingItemId { get; private set; }
    public int Position { get; private set; }
    public string Name { get; private set; } = null!;
    public decimal Quantity { get; private set; }
    public string Unit { get; private set; } = null!;

    internal static QuotationRequestItem Create(
        Guid quotationRequestId,
        int position,
        QuotationRequestItemSnapshot snapshot)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(quotationRequestId, Guid.Empty);
        ArgumentOutOfRangeException.ThrowIfEqual(snapshot.SourceShoppingItemId, Guid.Empty);
        ArgumentOutOfRangeException.ThrowIfNegative(position);
        ArgumentException.ThrowIfNullOrWhiteSpace(snapshot.Name);
        ArgumentException.ThrowIfNullOrWhiteSpace(snapshot.Unit);
        if (snapshot.Quantity <= 0) throw new ArgumentOutOfRangeException(nameof(snapshot));

        return new QuotationRequestItem(
            Guid.NewGuid(),
            quotationRequestId,
            snapshot.SourceShoppingItemId,
            position,
            snapshot.Name.Trim(),
            snapshot.Quantity,
            snapshot.Unit.Trim());
    }
}
