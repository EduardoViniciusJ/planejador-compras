using System.ComponentModel.DataAnnotations;

namespace PlanejadorCompras.Application.Features.ShoppingLists.Contracts;

public sealed record ShoppingListRequestDto(
    [Required]
    [MinLength(1)]
    [MaxLength(100)]
    string Name,
    [MinLength(1)]
    [MaxLength(500)]
    string? Description);
