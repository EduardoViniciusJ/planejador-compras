using System.ComponentModel.DataAnnotations;
using PlanejadorCompras.Domain.Rules;

namespace PlanejadorCompras.Application.Features.Suppliers.Contracts;

public sealed record SupplierRequestDto(
    [Required]
    [MinLength(1)]
    [MaxLength(SupplierRules.NameMaxLength)]
    string Name,
    [MaxLength(18)]
    string? Cnpj = null,
    SupplierAddressRequestDto? Address = null,
    SupplierContactRequestDto? Contact = null);

public sealed record SupplierAddressRequestDto(
    [MaxLength(SupplierRules.StreetMaxLength)]
    string? Street = null,
    [MaxLength(SupplierRules.CityMaxLength)]
    string? City = null,
    [MaxLength(9)]
    string? PostalCode = null);

public sealed record SupplierContactRequestDto(
    [EmailAddress]
    [MaxLength(SupplierRules.EmailMaxLength)]
    string? Email = null,
    [MaxLength(20)]
    string? Phone = null);
