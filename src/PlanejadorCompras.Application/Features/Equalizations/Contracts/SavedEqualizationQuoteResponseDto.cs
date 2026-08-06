namespace PlanejadorCompras.Application.Features.Equalizations.Contracts;

public sealed record SavedEqualizationQuoteResponseDto(
    Guid SupplierId,
    string SupplierName,
    decimal UnitPrice,
    decimal TotalPrice,
    bool IsLowest);
