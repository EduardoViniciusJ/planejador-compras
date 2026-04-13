namespace PlanejadorCompras.Domain.Repositories.ShoppingList;

public interface IShoppingListRepository
{
    Task<Entities.ShoppingList?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Entities.ShoppingList>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(Entities.ShoppingList shoppingList, CancellationToken cancellationToken = default);
    Task UpdateAsync(Entities.ShoppingList shoppingList, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
