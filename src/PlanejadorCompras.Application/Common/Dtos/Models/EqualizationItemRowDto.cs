namespace PlanejadorCompras.Application.Common.Dtos.Models;

public record EqualizationItemRowDto(
    Guid ShoppingItemId,
    string ItemName,
    decimal Quantity,
    string Unit,
    IEnumerable<EqualizationQuoteDto> Quotes
);
