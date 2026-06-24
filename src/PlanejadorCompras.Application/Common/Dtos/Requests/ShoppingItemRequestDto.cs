using System.ComponentModel.DataAnnotations;

namespace PlanejadorCompras.Application.Common.Dtos.Requests;

public sealed record ShoppingItemRequestDto(
    [Required]
    Guid ShoppingListId,
    [Required]
    [MinLength(1)]
    [MaxLength(100)]
    string Name,
    [Required]
    [Range(0.01, double.MaxValue)]
    decimal Quantity,
    [Required]
    [MinLength(1)]
    [MaxLength(20)]
    string Unit);
