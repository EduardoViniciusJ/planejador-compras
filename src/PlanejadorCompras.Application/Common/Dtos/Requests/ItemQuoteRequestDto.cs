using System.ComponentModel.DataAnnotations;

namespace PlanejadorCompras.Application.Common.Dtos.Requests;

public sealed record ItemQuoteRequestDto(
    [property: Required]
    Guid ShoppingItemId,
    [property: Required]
    [property: MinLength(1)]
    [property: MaxLength(100)]
    string SupplierName,
    [property: Required]
    [property: Range(0, double.MaxValue)]
    decimal UnitPrice);
