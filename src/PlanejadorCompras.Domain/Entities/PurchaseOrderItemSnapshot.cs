namespace PlanejadorCompras.Domain.Entities;

public sealed record PurchaseOrderItemSnapshot(
    Guid SourceShoppingItemId,
    string Name,
    decimal Quantity,
    string Unit,
    decimal UnitPrice);
