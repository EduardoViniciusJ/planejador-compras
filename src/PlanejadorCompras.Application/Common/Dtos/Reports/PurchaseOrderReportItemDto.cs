namespace PlanejadorCompras.Application.Common.Dtos.Reports;

public sealed record PurchaseOrderReportItemDto(
    string Name,
    decimal Quantity,
    string Unit,
    decimal UnitPrice,
    decimal TotalPrice);
