using System.ComponentModel.DataAnnotations;

namespace PlanejadorCompras.Application.Common.Dtos.Requests;

public sealed record ItemQuoteRequestDto(
    [Required]
    Guid ShoppingItemId,
    [Required]
    Guid SupplierId,
    [Required]
    [Range(0, double.MaxValue)]
    decimal UnitPrice);
