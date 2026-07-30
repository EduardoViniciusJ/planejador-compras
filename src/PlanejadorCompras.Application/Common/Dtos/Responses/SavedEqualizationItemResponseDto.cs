namespace PlanejadorCompras.Application.Common.Dtos.Responses;

public sealed record SavedEqualizationItemResponseDto(
    Guid ShoppingItemId,
    string ItemName,
    decimal Quantity,
    string Unit,
    IReadOnlyCollection<SavedEqualizationQuoteResponseDto> Quotes);
