using Microsoft.EntityFrameworkCore;
using PlanejadorCompras.Application.Common.Dtos.Models;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Infrastructure.Persistence;

namespace PlanejadorCompras.Infrastructure.Queries;

public sealed class ShoppingListOverviewQuery : IShoppingListOverviewQuery
{
    private readonly PlanejadorComprasDbContext _context;

    public ShoppingListOverviewQuery(PlanejadorComprasDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ShoppingListOverviewDto>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(userId, Guid.Empty);

        var lists = await _context.ShoppingLists
            .AsNoTracking()
            .Where(list => list.UserId == userId)
            .OrderByDescending(list => list.CreatedAt)
            .ToListAsync(cancellationToken);

        if (lists.Count == 0)
        {
            return Array.Empty<ShoppingListOverviewDto>();
        }

        var listIds = lists.Select(list => list.Id).ToArray();
        var items = await _context.ShoppingItems
            .AsNoTracking()
            .Where(item => listIds.Contains(item.ShoppingListId))
            .Select(item => new
            {
                item.ShoppingListId,
                item.Quantity,
                MinimumUnitPrice = _context.ItemQuotes
                    .Where(quote =>
                        quote.ShoppingItemId == item.Id &&
                        _context.ShoppingListSuppliers.Any(link =>
                            link.ShoppingListId == item.ShoppingListId &&
                            link.SupplierId == quote.SupplierId))
                    .Select(quote => (decimal?)quote.UnitPrice)
                    .Min()
            })
            .ToListAsync(cancellationToken);

        var itemsByListId = items.ToLookup(item => item.ShoppingListId);

        return lists.Select(list =>
        {
            var listItems = itemsByListId[list.Id].ToList();

            return new ShoppingListOverviewDto(
                list.Id,
                list.Name,
                list.Description,
                list.CreatedAt,
                listItems.Count,
                listItems.Count(item => item.MinimumUnitPrice.HasValue),
                listItems.Sum(item => item.Quantity * (item.MinimumUnitPrice ?? 0m)));
        }).ToList();
    }
}
