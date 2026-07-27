namespace PlanejadorCompras.Application.Common.Dtos.Reports;

public sealed record ShoppingListReportPendingItemDto(
    Guid ShoppingItemId,
    string ItemName,
    IReadOnlyCollection<Guid> MissingSupplierIds,
    IReadOnlyCollection<string> MissingSupplierNames);
