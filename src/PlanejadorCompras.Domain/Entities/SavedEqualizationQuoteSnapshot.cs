namespace PlanejadorCompras.Domain.Entities;

public sealed record SavedEqualizationQuoteSnapshot(
    Guid SourceSupplierId,
    string SupplierName,
    decimal UnitPrice,
    bool IsLowest);
