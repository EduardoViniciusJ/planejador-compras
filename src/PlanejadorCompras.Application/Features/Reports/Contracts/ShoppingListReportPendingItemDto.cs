namespace PlanejadorCompras.Application.Features.Reports.Contracts;

public sealed record ShoppingListReportPendingItemDto(
    Guid ShoppingItemId,
    string ItemName,
    IReadOnlyCollection<Guid> MissingSupplierIds,
    IReadOnlyCollection<string> MissingSupplierNames);
