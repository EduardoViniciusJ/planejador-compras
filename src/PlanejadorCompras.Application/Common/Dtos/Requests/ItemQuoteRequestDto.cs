using System.ComponentModel.DataAnnotations;

namespace PlanejadorCompras.Application.Common.Dtos.Requests;

public sealed record ItemQuoteRequestDto(
    [Required]
    Guid ShoppingItemId,
    [Required]
    [MinLength(1)]
    [MaxLength(100)]
    string SupplierName,
    [Required]
    [Range(0, double.MaxValue)]
    decimal UnitPrice);
