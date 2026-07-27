namespace PlanejadorCompras.Application.Common.Dtos.Reports;

public sealed record ShoppingListReportQuoteDto(
    Guid SupplierId,
    string SupplierName,
    decimal UnitPrice,
    decimal TotalPrice,
    bool IsLowestPrice);
