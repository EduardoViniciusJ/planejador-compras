namespace PlanejadorCompras.Domain.Repositories.ShoppingListSupplier;

public interface IShoppingListSupplierRepository
{
    Task<List<Entities.Supplier>> GetSuppliersAsync(
        Guid shoppingListId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        Guid shoppingListId,
        Guid supplierId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Entities.ShoppingListSupplier shoppingListSupplier,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid shoppingListId,
        Guid supplierId,
        CancellationToken cancellationToken = default);
}
