using System.ComponentModel.DataAnnotations;
using PlanejadorCompras.Domain.Rules;

namespace PlanejadorCompras.Application.Features.ItemQuotes.Contracts;

public sealed record ItemQuoteRequestDto(
    [Required]
    Guid ShoppingItemId,
    [Required]
    Guid SupplierId,
    [Required]
    [Range(
        typeof(decimal),
        ItemQuoteRules.MinimumUnitPriceText,
        ItemQuoteRules.MaximumUnitPriceText)]
    decimal UnitPrice);
