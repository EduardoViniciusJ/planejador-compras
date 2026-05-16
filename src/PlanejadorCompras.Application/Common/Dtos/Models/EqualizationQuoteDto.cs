namespace PlanejadorCompras.Application.Common.Dtos.Models;

public record EqualizationQuoteDto(
    string SupplierName,
    decimal UnitPrice,
    decimal TotalPrice
);
