using System.ComponentModel.DataAnnotations;

namespace PlanejadorCompras.Application.Common.Dtos.Requests;

public sealed record SupplierRequestDto(
    [Required]
    [MinLength(1)]
    [MaxLength(100)]
    string Name,
    [MaxLength(18)]
    string? Cnpj = null,
    SupplierAddressRequestDto? Address = null,
    SupplierContactRequestDto? Contact = null);

public sealed record SupplierAddressRequestDto(
    [MaxLength(200)]
    string? Street = null,
    [MaxLength(100)]
    string? City = null,
    [MaxLength(9)]
    string? PostalCode = null);

public sealed record SupplierContactRequestDto(
    [EmailAddress]
    [MaxLength(254)]
    string? Email = null,
    [MaxLength(20)]
    string? Phone = null);
