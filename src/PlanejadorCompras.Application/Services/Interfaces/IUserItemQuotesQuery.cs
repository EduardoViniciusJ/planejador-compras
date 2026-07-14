using PlanejadorCompras.Application.Common.Dtos.Models;

namespace PlanejadorCompras.Application.Services.Interfaces;

public interface IUserItemQuotesQuery
{
    Task<IReadOnlyCollection<UserItemQuoteDto>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
