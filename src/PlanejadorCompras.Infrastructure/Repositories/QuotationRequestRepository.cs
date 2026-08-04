using Microsoft.EntityFrameworkCore;
using PlanejadorCompras.Domain.Repositories.QuotationRequest;
using PlanejadorCompras.Infrastructure.Persistence;
using QuotationRequestEntity = PlanejadorCompras.Domain.Entities.QuotationRequest;

namespace PlanejadorCompras.Infrastructure.Repositories;

public sealed class QuotationRequestRepository(PlanejadorComprasDbContext context)
    : IQuotationRequestRepository
{
    public Task<QuotationRequestEntity?> GetByIdForUserAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        context.QuotationRequests
            .AsNoTracking()
            .Include(request => request.Items)
            .FirstOrDefaultAsync(request => request.Id == id && request.UserId == userId, cancellationToken);

    public Task<List<QuotationRequestEntity>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default) =>
        context.QuotationRequests
            .AsNoTracking()
            .Include(request => request.Items)
            .Where(request => request.UserId == userId)
            .OrderByDescending(request => request.CreatedAtUtc)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(
        QuotationRequestEntity request,
        CancellationToken cancellationToken = default) =>
        await context.QuotationRequests.AddAsync(request, cancellationToken);
}
