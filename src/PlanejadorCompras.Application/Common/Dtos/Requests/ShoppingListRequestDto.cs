using System.ComponentModel.DataAnnotations;

namespace PlanejadorCompras.Application.Common.Dtos.Requests;

public sealed record ShoppingListRequestDto(
    [Required]
    [MinLength(1)]
    [MaxLength(100)]
    string Name,
    [MinLength(1)]
    [MaxLength(500)]
    string? Description);
