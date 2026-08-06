using PlanejadorCompras.Application.Features.ItemQuotes.Contracts;

namespace PlanejadorCompras.Application.Services.Interfaces;

public interface IUserItemQuotesQuery
{
    Task<IReadOnlyCollection<UserItemQuoteDto>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
