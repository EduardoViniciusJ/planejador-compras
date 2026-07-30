namespace PlanejadorCompras.Domain.Entities;

public sealed record SavedEqualizationItemSnapshot(
    Guid SourceShoppingItemId,
    int Position,
    string Name,
    decimal Quantity,
    string Unit,
    IReadOnlyCollection<SavedEqualizationQuoteSnapshot> Quotes);
