namespace PlanejadorCompras.Application.Common.Dtos.Reports;

public sealed record ShoppingListReportDataDto(
    Guid ShoppingListId,
    string Name,
    string? Description,
    DateTime CreatedAt,
    DateTimeOffset GeneratedAt,
    ShoppingListReportSummaryDto Summary,
    IReadOnlyCollection<ShoppingListReportSupplierDto> Suppliers,
    IReadOnlyCollection<ShoppingListReportItemDto> Items,
    IReadOnlyCollection<ShoppingListReportPendingItemDto> PendingItems);
