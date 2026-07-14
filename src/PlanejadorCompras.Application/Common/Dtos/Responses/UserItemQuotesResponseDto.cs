using PlanejadorCompras.Application.Common.Dtos.Models;

namespace PlanejadorCompras.Application.Common.Dtos.Responses;

public sealed record UserItemQuotesResponseDto(
    IReadOnlyCollection<UserItemQuoteDto> Quotes);
