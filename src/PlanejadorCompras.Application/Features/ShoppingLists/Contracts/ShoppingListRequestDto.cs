using System.ComponentModel.DataAnnotations;
using PlanejadorCompras.Domain.Rules;

namespace PlanejadorCompras.Application.Features.ShoppingLists.Contracts;

public sealed record ShoppingListRequestDto(
    [Required]
    [MinLength(1)]
    [MaxLength(ShoppingListRules.NameMaxLength)]
    string Name,
    [MinLength(1)]
    [MaxLength(ShoppingListRules.DescriptionMaxLength)]
    string? Description);
