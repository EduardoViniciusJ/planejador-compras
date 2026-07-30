namespace PlanejadorCompras.Application.Common.Dtos.Responses;

public sealed record SavedEqualizationQuoteResponseDto(
    Guid SupplierId,
    string SupplierName,
    decimal UnitPrice,
    decimal TotalPrice,
    bool IsLowest);
