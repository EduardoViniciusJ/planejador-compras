namespace PlanejadorCompras.Application.Features.Equalizations.Contracts;

public record EqualizationQuoteDto(
    Guid SupplierId,
    string SupplierName,
    decimal UnitPrice,
    decimal TotalPrice
);
