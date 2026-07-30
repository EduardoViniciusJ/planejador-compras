namespace PlanejadorCompras.Domain.Entities;

public sealed class SavedEqualizationItem
{
    private readonly List<SavedEqualizationQuote> _quotes = [];

    private SavedEqualizationItem(
        Guid id,
        Guid savedEqualizationId,
        Guid sourceShoppingItemId,
        int position,
        string name,
        decimal quantity,
        string unit)
    {
        Id = id;
        SavedEqualizationId = savedEqualizationId;
        SourceShoppingItemId = sourceShoppingItemId;
        Position = position;
        Name = name;
        Quantity = quantity;
        Unit = unit;
    }

    public Guid Id { get; private set; }

    public Guid SavedEqualizationId { get; private set; }

    public Guid SourceShoppingItemId { get; private set; }

    public int Position { get; private set; }

    public string Name { get; private set; } = null!;

    public decimal Quantity { get; private set; }

    public string Unit { get; private set; } = null!;

    public IReadOnlyCollection<SavedEqualizationQuote> Quotes => _quotes.AsReadOnly();

    internal static SavedEqualizationItem Create(
        Guid savedEqualizationId,
        SavedEqualizationItemSnapshot snapshot)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(savedEqualizationId, Guid.Empty);
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentOutOfRangeException.ThrowIfEqual(snapshot.SourceShoppingItemId, Guid.Empty);
        ArgumentException.ThrowIfNullOrWhiteSpace(snapshot.Name);
        ArgumentException.ThrowIfNullOrWhiteSpace(snapshot.Unit);
        ArgumentNullException.ThrowIfNull(snapshot.Quotes);

        if (snapshot.Position < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(snapshot),
                "Equalization item position cannot be negative.");
        }

        if (snapshot.Quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(snapshot),
                "Equalization item quantity must be greater than zero.");
        }

        var item = new SavedEqualizationItem(
            Guid.NewGuid(),
            savedEqualizationId,
            snapshot.SourceShoppingItemId,
            snapshot.Position,
            snapshot.Name.Trim(),
            snapshot.Quantity,
            snapshot.Unit.Trim());

        foreach (var quote in snapshot.Quotes)
        {
            item._quotes.Add(SavedEqualizationQuote.Create(item.Id, quote));
        }

        return item;
    }
}
