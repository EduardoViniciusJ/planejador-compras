using System.ComponentModel.DataAnnotations;

namespace PlanejadorCompras.Application.Common.Dtos.Requests;

public sealed record ShoppingListRequestDto(
    [property: Required]
    [property: MinLength(1)]
    [property: MaxLength(100)]
    string Name,
    [property: MaxLength(500)]
    string? Description);
