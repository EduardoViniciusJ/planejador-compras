using Microsoft.EntityFrameworkCore;
using PlanejadorCompras.Domain.Entities;
using PlanejadorCompras.Domain.Repositories.ShoppingListSupplier;
using PlanejadorCompras.Infrastructure.Persistence;

namespace PlanejadorCompras.Infrastructure.Repositories;

public sealed class ShoppingListSupplierRepository(PlanejadorComprasDbContext context)
    : IShoppingListSupplierRepository
{
    public Task<List<Supplier>> GetSuppliersAsync(
        Guid shoppingListId,
        CancellationToken cancellationToken = default) =>
        context.ShoppingListSuppliers
            .AsNoTracking()
            .Where(link => link.ShoppingListId == shoppingListId)
            .Join(
                context.Suppliers.AsNoTracking(),
                link => link.SupplierId,
                supplier => supplier.Id,
                (_, supplier) => supplier)
            .OrderBy(supplier => supplier.Name)
            .ToListAsync(cancellationToken);

    public Task<bool> ExistsAsync(
        Guid shoppingListId,
        Guid supplierId,
        CancellationToken cancellationToken = default) =>
        context.ShoppingListSuppliers.AnyAsync(
            link => link.ShoppingListId == shoppingListId && link.SupplierId == supplierId,
            cancellationToken);

    public async Task AddAsync(
        ShoppingListSupplier shoppingListSupplier,
        CancellationToken cancellationToken = default) =>
        await context.ShoppingListSuppliers.AddAsync(shoppingListSupplier, cancellationToken);

    public async Task<bool> DeleteAsync(
        Guid shoppingListId,
        Guid supplierId,
        CancellationToken cancellationToken = default)
    {
        var link = await context.ShoppingListSuppliers.FirstOrDefaultAsync(
            candidate => candidate.ShoppingListId == shoppingListId &&
                         candidate.SupplierId == supplierId,
            cancellationToken);
        if (link is null) return false;

        context.ShoppingListSuppliers.Remove(link);
        return true;
    }
}
