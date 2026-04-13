using Microsoft.EntityFrameworkCore;
using PlanejadorCompras.Domain.Entities;
using PlanejadorCompras.Domain.Repositories.ShoppingList;
using PlanejadorCompras.Infrastructure.Persistence;

namespace PlanejadorCompras.Infrastructure.Repositories;

public sealed class ShoppingListRepository : IShoppingListRepository
{
    private readonly PlanejadorComprasDbContext _context;

    public ShoppingListRepository(PlanejadorComprasDbContext context)
    {
        _context = context;
    }

    public async Task<ShoppingList?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ShoppingLists.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<List<ShoppingList>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.ShoppingLists
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ShoppingList shoppingList, CancellationToken cancellationToken = default)
    {
        await _context.ShoppingLists.AddAsync(shoppingList, cancellationToken);
    }

    public async Task UpdateAsync(ShoppingList shoppingList, CancellationToken cancellationToken = default)
    {
        _context.ShoppingLists.Update(shoppingList);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var shoppingList = await GetByIdAsync(id, cancellationToken);
        if (shoppingList is not null)
        {
            _context.ShoppingLists.Remove(shoppingList);
        }
    }
}
