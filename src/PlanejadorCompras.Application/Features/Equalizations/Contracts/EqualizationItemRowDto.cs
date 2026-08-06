namespace PlanejadorCompras.Application.Features.Equalizations.Contracts;

public record EqualizationItemRowDto(
    Guid ShoppingItemId,
    string ItemName,
    decimal Quantity,
    string Unit,
    IEnumerable<EqualizationQuoteDto> Quotes
);
