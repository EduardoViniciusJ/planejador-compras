using Microsoft.EntityFrameworkCore;
using PlanejadorCompras.Domain.Entities;
using PlanejadorCompras.Domain.Repositories.Equalization;
using PlanejadorCompras.Infrastructure.Persistence;

namespace PlanejadorCompras.Infrastructure.Repositories;

public sealed class SavedEqualizationRepository(PlanejadorComprasDbContext context)
    : ISavedEqualizationRepository
{
    public Task<SavedEqualization?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        IncludeSnapshot(context.SavedEqualizations)
            .FirstOrDefaultAsync(equalization => equalization.Id == id, cancellationToken);

    public Task<SavedEqualization?> GetByRequestIdAsync(
        Guid userId,
        Guid requestId,
        CancellationToken cancellationToken = default) =>
        IncludeSnapshot(context.SavedEqualizations)
            .FirstOrDefaultAsync(
                equalization =>
                    equalization.UserId == userId
                    && equalization.RequestId == requestId,
                cancellationToken);

    public async Task<(List<SavedEqualization> Items, int TotalCount)> SearchByUserIdAsync(
        Guid userId,
        string? searchTerm,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = context.SavedEqualizations
            .AsNoTracking()
            .Where(equalization => equalization.UserId == userId);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(equalization =>
                equalization.Code.Contains(term)
                || equalization.ShoppingListName.Contains(term)
                || equalization.CreatedByName.Contains(term)
                || equalization.CreatedByEmail.Contains(term)
                || (equalization.BestCompleteSupplierName != null
                    && equalization.BestCompleteSupplierName.Contains(term)));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await IncludeSnapshot(query)
            .OrderByDescending(equalization => equalization.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<bool> CodeExistsAsync(
        string code,
        CancellationToken cancellationToken = default) =>
        context.SavedEqualizations.AnyAsync(
            equalization => equalization.Code == code,
            cancellationToken);

    public async Task AddAsync(
        SavedEqualization equalization,
        CancellationToken cancellationToken = default)
    {
        await context.SavedEqualizations.AddAsync(equalization, cancellationToken);
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var equalization = await GetByIdAsync(id, cancellationToken);
        if (equalization is null)
        {
            return false;
        }

        context.SavedEqualizations.Remove(equalization);
        return true;
    }

    private static IQueryable<SavedEqualization> IncludeSnapshot(
        IQueryable<SavedEqualization> query) =>
        query
            .Include(equalization => equalization.Items)
            .ThenInclude(item => item.Quotes)
            .AsSplitQuery();
}
