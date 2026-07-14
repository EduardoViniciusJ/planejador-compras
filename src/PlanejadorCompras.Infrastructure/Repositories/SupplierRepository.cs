using Microsoft.EntityFrameworkCore;
using PlanejadorCompras.Domain.Entities;
using PlanejadorCompras.Domain.Repositories.Supplier;
using PlanejadorCompras.Infrastructure.Persistence;

namespace PlanejadorCompras.Infrastructure.Repositories;

public sealed class SupplierRepository(PlanejadorComprasDbContext context) : ISupplierRepository
{
    public Task<Supplier?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Suppliers.FirstOrDefaultAsync(supplier => supplier.Id == id, cancellationToken);

    public Task<List<Supplier>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default)
    {
        var supplierIds = ids.Distinct().ToArray();
        return context.Suppliers
            .AsNoTracking()
            .Where(supplier => supplierIds.Contains(supplier.Id))
            .ToListAsync(cancellationToken);
    }

    public Task<List<Supplier>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        context.Suppliers
            .AsNoTracking()
            .Where(supplier => supplier.UserId == userId)
            .OrderBy(supplier => supplier.Name)
            .ToListAsync(cancellationToken);

    public Task<bool> ExistsByNameAsync(
        Guid userId,
        string name,
        Guid? excludingId = null,
        CancellationToken cancellationToken = default) =>
        context.Suppliers.AnyAsync(
            supplier => supplier.UserId == userId &&
                        supplier.Name == name.Trim() &&
                        (!excludingId.HasValue || supplier.Id != excludingId.Value),
            cancellationToken);

    public Task<bool> HasQuotesAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.ItemQuotes.AnyAsync(quote => quote.SupplierId == id, cancellationToken);

    public async Task AddAsync(Supplier supplier, CancellationToken cancellationToken = default) =>
        await context.Suppliers.AddAsync(supplier, cancellationToken);

    public Task UpdateAsync(Supplier supplier, CancellationToken cancellationToken = default)
    {
        context.Suppliers.Update(supplier);
        return Task.CompletedTask;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var supplier = await GetByIdAsync(id, cancellationToken);
        if (supplier is null) return false;

        context.Suppliers.Remove(supplier);
        return true;
    }
}
