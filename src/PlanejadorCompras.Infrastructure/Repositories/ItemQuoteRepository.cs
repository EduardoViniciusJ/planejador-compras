using Microsoft.EntityFrameworkCore;
using PlanejadorCompras.Domain.Entities;
using PlanejadorCompras.Domain.Repositories.ItemQuote;
using PlanejadorCompras.Infrastructure.Persistence;

namespace PlanejadorCompras.Infrastructure.Repositories;

public sealed class ItemQuoteRepository : IItemQuoteRepository
{
    private readonly PlanejadorComprasDbContext _context;

    public ItemQuoteRepository(PlanejadorComprasDbContext context)
    {
        _context = context;
    }

    public async Task<ItemQuote?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ItemQuotes.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<List<ItemQuote>> GetByShoppingItemIdAsync(Guid shoppingItemId, CancellationToken cancellationToken = default)
    {
        return await _context.ItemQuotes
            .Where(quote =>
                quote.ShoppingItemId == shoppingItemId &&
                _context.ShoppingItems.Any(item =>
                    item.Id == shoppingItemId &&
                    _context.ShoppingListSuppliers.Any(link =>
                        link.ShoppingListId == item.ShoppingListId &&
                        link.SupplierId == quote.SupplierId)))
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ItemQuote itemQuote, CancellationToken cancellationToken = default)
    {
        await _context.ItemQuotes.AddAsync(itemQuote, cancellationToken);
    }

    public async Task UpdateAsync(ItemQuote itemQuote, CancellationToken cancellationToken = default)
    {
        _context.ItemQuotes.Update(itemQuote);
        await Task.CompletedTask;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var itemQuote = await GetByIdAsync(id, cancellationToken);
        if (itemQuote is not null)
        {
            _context.ItemQuotes.Remove(itemQuote);
            return true;
        }

        return false;
    }

    public async Task<List<ItemQuote>> GetByShoppingListIdAsync(Guid shoppingListId, CancellationToken cancellationToken = default)
    {
        return await _context.ItemQuotes
            .Where(quote =>
                _context.ShoppingItems.Any(item =>
                    item.Id == quote.ShoppingItemId &&
                    item.ShoppingListId == shoppingListId) &&
                _context.ShoppingListSuppliers.Any(link =>
                    link.ShoppingListId == shoppingListId &&
                    link.SupplierId == quote.SupplierId))
            .ToListAsync(cancellationToken);
    }
}
