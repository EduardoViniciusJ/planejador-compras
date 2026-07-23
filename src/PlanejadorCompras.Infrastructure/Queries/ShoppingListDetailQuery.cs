using Microsoft.EntityFrameworkCore;
using PlanejadorCompras.Application.Common.Dtos.Models;
using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Infrastructure.Persistence;

namespace PlanejadorCompras.Infrastructure.Queries;

public sealed class ShoppingListDetailQuery : IShoppingListDetailQuery
{
    private readonly PlanejadorComprasDbContext _context;

    public ShoppingListDetailQuery(PlanejadorComprasDbContext context)
    {
        _context = context;
    }

    public async Task<ShoppingListDetailResponseDto?> GetByIdAsync(
        Guid userId,
        Guid shoppingListId,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(userId, Guid.Empty);
        ArgumentOutOfRangeException.ThrowIfEqual(shoppingListId, Guid.Empty);

        var shoppingList = await _context.ShoppingLists
            .AsNoTracking()
            .Where(list => list.Id == shoppingListId && list.UserId == userId)
            .Select(list => new
            {
                list.Id,
                list.Name,
                list.Description,
                list.CreatedAt
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (shoppingList is null)
        {
            return null;
        }

        var items = await _context.ShoppingItems
            .AsNoTracking()
            .Where(item => item.ShoppingListId == shoppingListId)
            .OrderBy(item => item.CreatedAt)
            .Select(item => new ShoppingListDetailItemDto(
                item.Id,
                item.Name,
                item.Quantity,
                item.Unit,
                item.CreatedAt,
                _context.ItemQuotes.Count(quote =>
                    quote.ShoppingItemId == item.Id &&
                    _context.ShoppingListSuppliers.Any(link =>
                        link.ShoppingListId == shoppingListId &&
                        link.SupplierId == quote.SupplierId)),
                _context.ItemQuotes
                    .Where(quote =>
                        quote.ShoppingItemId == item.Id &&
                        _context.ShoppingListSuppliers.Any(link =>
                            link.ShoppingListId == shoppingListId &&
                            link.SupplierId == quote.SupplierId))
                    .Select(quote => (decimal?)quote.UnitPrice)
                    .Min()))
            .ToListAsync(cancellationToken);

        var quotedItems = items.Count(item => item.QuoteCount > 0);
        var totalEstimated = items.Sum(item => (item.BestUnitPrice ?? 0m) * item.Quantity);

        return new ShoppingListDetailResponseDto(
            shoppingList.Id,
            shoppingList.Name,
            shoppingList.Description,
            shoppingList.CreatedAt,
            items.Count,
            quotedItems,
            totalEstimated,
            items);
    }
}
