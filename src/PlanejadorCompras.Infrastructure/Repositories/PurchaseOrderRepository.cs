using Microsoft.EntityFrameworkCore;
using PlanejadorCompras.Domain.Entities;
using PlanejadorCompras.Domain.Repositories.PurchaseOrder;
using PlanejadorCompras.Infrastructure.Persistence;
using PurchaseOrderEntity = PlanejadorCompras.Domain.Entities.PurchaseOrder;

namespace PlanejadorCompras.Infrastructure.Repositories;

public sealed class PurchaseOrderRepository(PlanejadorComprasDbContext context)
    : IPurchaseOrderRepository
{
    public Task<PurchaseOrderEntity?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        context.PurchaseOrders
            .Include(order => order.Items)
            .FirstOrDefaultAsync(order => order.Id == id, cancellationToken);

    public Task<List<PurchaseOrderEntity>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default) =>
        context.PurchaseOrders
            .AsNoTracking()
            .Include(order => order.Items)
            .Where(order => order.UserId == userId)
            .OrderByDescending(order => order.CreatedAtUtc)
            .ToListAsync(cancellationToken);

    public Task<bool> ExistsForSourceAsync(
        Guid userId,
        Guid shoppingListId,
        Guid supplierId,
        CancellationToken cancellationToken = default) =>
        context.PurchaseOrders.AnyAsync(
            order =>
                order.UserId == userId
                && order.SourceShoppingListId == shoppingListId
                && order.SupplierId == supplierId
                && order.Status != PurchaseOrderStatus.Cancelled,
            cancellationToken);

    public Task<bool> CodeExistsAsync(
        string code,
        CancellationToken cancellationToken = default) =>
        context.PurchaseOrders.AnyAsync(
            order => order.Code == code,
            cancellationToken);

    public async Task AddAsync(
        PurchaseOrderEntity purchaseOrder,
        CancellationToken cancellationToken = default)
    {
        await context.PurchaseOrders.AddAsync(purchaseOrder, cancellationToken);
    }
}
