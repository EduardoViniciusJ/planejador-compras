namespace PlanejadorCompras.Domain.Repositories.ShoppingItem;

public interface IShoppingItemRepository
{
    Task<Entities.ShoppingItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Entities.ShoppingItem>> GetByShoppingListIdAsync(Guid shoppingListId, CancellationToken cancellationToken = default);
    Task AddAsync(Entities.ShoppingItem shoppingItem, CancellationToken cancellationToken = default);
    Task UpdateAsync(Entities.ShoppingItem shoppingItem, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
