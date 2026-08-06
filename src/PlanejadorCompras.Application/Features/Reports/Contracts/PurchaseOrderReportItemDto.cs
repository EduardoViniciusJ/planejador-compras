namespace PlanejadorCompras.Application.Features.Reports.Contracts;

public sealed record PurchaseOrderReportItemDto(
    string Name,
    decimal Quantity,
    string Unit,
    decimal UnitPrice,
    decimal TotalPrice);
