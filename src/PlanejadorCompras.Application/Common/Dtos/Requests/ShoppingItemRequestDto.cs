using System.ComponentModel.DataAnnotations;

namespace PlanejadorCompras.Application.Common.Dtos.Requests;

public sealed record ShoppingItemRequestDto(
    [property: Required]
    Guid ShoppingListId,
    [property: Required]
    [property: MinLength(1)]
    [property: MaxLength(100)]
    string Name,
    [property: Required]
    [property: Range(0.01, double.MaxValue)]
    decimal Quantity,
    [property: Required]
    [property: MinLength(1)]
    [property: MaxLength(20)]
    string Unit);
