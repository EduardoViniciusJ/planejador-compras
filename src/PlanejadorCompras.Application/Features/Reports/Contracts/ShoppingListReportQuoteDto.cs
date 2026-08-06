namespace PlanejadorCompras.Application.Features.Reports.Contracts;

public sealed record ShoppingListReportQuoteDto(
    Guid SupplierId,
    string SupplierName,
    decimal UnitPrice,
    decimal TotalPrice,
    bool IsLowestPrice);
