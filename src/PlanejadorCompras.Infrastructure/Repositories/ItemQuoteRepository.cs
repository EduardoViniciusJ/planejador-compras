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
            .Where(x => x.ShoppingItemId == shoppingItemId)
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

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var itemQuote = await GetByIdAsync(id, cancellationToken);
        if (itemQuote is not null)
        {
            _context.ItemQuotes.Remove(itemQuote);
        }
    }
}
