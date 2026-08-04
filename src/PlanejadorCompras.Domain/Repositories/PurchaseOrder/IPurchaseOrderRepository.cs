using PlanejadorCompras.Domain.Entities;

namespace PlanejadorCompras.Domain.Repositories.PurchaseOrder;

public interface IPurchaseOrderRepository
{
    Task<Entities.PurchaseOrder?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<List<Entities.PurchaseOrder>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsForSourceAsync(
        Guid userId,
        Guid shoppingListId,
        Guid supplierId,
        CancellationToken cancellationToken = default);

    Task<bool> CodeExistsAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Entities.PurchaseOrder purchaseOrder,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
