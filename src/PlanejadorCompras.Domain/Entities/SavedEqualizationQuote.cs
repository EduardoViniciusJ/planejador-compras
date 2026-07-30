namespace PlanejadorCompras.Domain.Entities;

public sealed class SavedEqualizationQuote
{
    private SavedEqualizationQuote(
        Guid id,
        Guid savedEqualizationItemId,
        Guid sourceSupplierId,
        string supplierName,
        decimal unitPrice,
        bool isLowest)
    {
        Id = id;
        SavedEqualizationItemId = savedEqualizationItemId;
        SourceSupplierId = sourceSupplierId;
        SupplierName = supplierName;
        UnitPrice = unitPrice;
        IsLowest = isLowest;
    }

    public Guid Id { get; private set; }

    public Guid SavedEqualizationItemId { get; private set; }

    public Guid SourceSupplierId { get; private set; }

    public string SupplierName { get; private set; } = null!;

    public decimal UnitPrice { get; private set; }

    public bool IsLowest { get; private set; }

    internal static SavedEqualizationQuote Create(
        Guid savedEqualizationItemId,
        SavedEqualizationQuoteSnapshot snapshot)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(savedEqualizationItemId, Guid.Empty);
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentOutOfRangeException.ThrowIfEqual(snapshot.SourceSupplierId, Guid.Empty);
        ArgumentException.ThrowIfNullOrWhiteSpace(snapshot.SupplierName);

        if (snapshot.UnitPrice < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(snapshot),
                "Equalization quote price cannot be negative.");
        }

        return new SavedEqualizationQuote(
            Guid.NewGuid(),
            savedEqualizationItemId,
            snapshot.SourceSupplierId,
            snapshot.SupplierName.Trim(),
            snapshot.UnitPrice,
            snapshot.IsLowest);
    }
}
