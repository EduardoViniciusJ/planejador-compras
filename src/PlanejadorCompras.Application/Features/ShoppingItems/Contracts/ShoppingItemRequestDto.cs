using System.ComponentModel.DataAnnotations;
using PlanejadorCompras.Domain.Rules;

namespace PlanejadorCompras.Application.Features.ShoppingItems.Contracts;

public sealed record ShoppingItemRequestDto(
    [Required]
    Guid ShoppingListId,
    [Required]
    [MinLength(1)]
    [MaxLength(ShoppingItemRules.NameMaxLength)]
    string Name,
    [Required]
    [Range(
        typeof(decimal),
        ShoppingItemRules.MinimumQuantityText,
        ShoppingItemRules.MaximumQuantityText,
        ParseLimitsInInvariantCulture = true)]
    decimal Quantity,
    [Required]
    [MinLength(1)]
    [MaxLength(ShoppingItemRules.UnitMaxLength)]
    string Unit);
