namespace PlanejadorCompras.Application.Common.Dtos.Models;

public record EqualizationQuoteDto(
    Guid SupplierId,
    string SupplierName,
    decimal UnitPrice,
    decimal TotalPrice
);
