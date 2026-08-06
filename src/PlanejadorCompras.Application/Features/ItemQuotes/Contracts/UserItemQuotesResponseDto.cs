
namespace PlanejadorCompras.Application.Features.ItemQuotes.Contracts;

public sealed record UserItemQuotesResponseDto(
    IReadOnlyCollection<UserItemQuoteDto> Quotes);
