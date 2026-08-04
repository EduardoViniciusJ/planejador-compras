namespace PlanejadorCompras.Domain.Repositories.QuotationRequest;

public interface IQuotationRequestRepository
{
    Task<Entities.QuotationRequest?> GetByIdForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<List<Entities.QuotationRequest>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Entities.QuotationRequest request,
        CancellationToken cancellationToken = default);
}
