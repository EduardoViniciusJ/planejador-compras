namespace PlanejadorCompras.Application.Features.Reports.Contracts;

public sealed record ShoppingListReportSupplierDto(
    Guid SupplierId,
    string Name,
    int QuotedItemCount,
    int MissingItemCount,
    bool HasCompleteCoverage,
    decimal QuotedTotal);
