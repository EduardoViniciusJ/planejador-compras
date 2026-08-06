namespace PlanejadorCompras.Application.Features.Reports.Contracts;

public sealed record ShoppingListReportItemDto(
    Guid ShoppingItemId,
    string Name,
    decimal Quantity,
    string Unit,
    decimal? LowestUnitPrice,
    decimal? LowestTotalPrice,
    IReadOnlyCollection<ShoppingListReportQuoteDto> Quotes);
