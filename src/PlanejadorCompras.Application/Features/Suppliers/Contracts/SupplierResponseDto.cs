namespace PlanejadorCompras.Application.Features.Suppliers.Contracts;

public sealed record SupplierResponseDto(
    Guid Id,
    string Name,
    string? Cnpj,
    SupplierAddressResponseDto? Address,
    SupplierContactResponseDto? Contact,
    DateTime CreatedAt);

public sealed record SupplierAddressResponseDto(
    string? Street,
    string? City,
    string? PostalCode);

public sealed record SupplierContactResponseDto(
    string? Email,
    string? Phone);
