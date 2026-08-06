namespace PlanejadorCompras.Application.Features.Equalizations.Contracts;

public sealed record SavedEqualizationItemResponseDto(
    Guid ShoppingItemId,
    string ItemName,
    decimal Quantity,
    string Unit,
    IReadOnlyCollection<SavedEqualizationQuoteResponseDto> Quotes);
