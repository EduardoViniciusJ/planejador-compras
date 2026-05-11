namespace PlanejadorCompras.Domain.Repositories.ItemQuote;

public interface IItemQuoteRepository
{
    Task<Entities.ItemQuote?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Entities.ItemQuote>> GetByShoppingItemIdAsync(Guid shoppingItemId, CancellationToken cancellationToken = default);
    Task AddAsync(Entities.ItemQuote itemQuote, CancellationToken cancellationToken = default);
    Task UpdateAsync(Entities.ItemQuote itemQuote, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
