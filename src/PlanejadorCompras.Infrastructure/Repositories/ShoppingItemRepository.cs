using Microsoft.EntityFrameworkCore;
using PlanejadorCompras.Domain.Entities;
using PlanejadorCompras.Domain.Repositories.ShoppingItem;
using PlanejadorCompras.Infrastructure.Persistence;

namespace PlanejadorCompras.Infrastructure.Repositories;

public sealed class ShoppingItemRepository : IShoppingItemRepository
{
    private readonly PlanejadorComprasDbContext _context;

    public ShoppingItemRepository(PlanejadorComprasDbContext context)
    {
        _context = context;
    }

    public async Task<ShoppingItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ShoppingItems.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<List<ShoppingItem>> GetByShoppingListIdAsync(Guid shoppingListId, CancellationToken cancellationToken = default)
    {
        return await _context.ShoppingItems
            .Where(x => x.ShoppingListId == shoppingListId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ShoppingItem shoppingItem, CancellationToken cancellationToken = default)
    {
        await _context.ShoppingItems.AddAsync(shoppingItem, cancellationToken);
    }

    public async Task UpdateAsync(ShoppingItem shoppingItem, CancellationToken cancellationToken = default)
    {
        _context.ShoppingItems.Update(shoppingItem);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var shoppingItem = await GetByIdAsync(id, cancellationToken);
        if (shoppingItem is not null)
        {
            _context.ShoppingItems.Remove(shoppingItem);
        }
    }
}
