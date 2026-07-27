namespace PlanejadorCompras.Application.Common.Dtos.Reports;

public sealed record ShoppingListReportSupplierDto(
    Guid SupplierId,
    string Name,
    int QuotedItemCount,
    int MissingItemCount,
    bool HasCompleteCoverage,
    decimal QuotedTotal);
