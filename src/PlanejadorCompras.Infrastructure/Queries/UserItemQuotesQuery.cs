using Microsoft.EntityFrameworkCore;
using PlanejadorCompras.Application.Common.Dtos.Models;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Infrastructure.Persistence;

namespace PlanejadorCompras.Infrastructure.Queries;

public sealed class UserItemQuotesQuery : IUserItemQuotesQuery
{
    private readonly PlanejadorComprasDbContext _context;

    public UserItemQuotesQuery(PlanejadorComprasDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<UserItemQuoteDto>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(userId, Guid.Empty);

        return await (
                from quote in _context.ItemQuotes.AsNoTracking()
                join item in _context.ShoppingItems.AsNoTracking()
                    on quote.ShoppingItemId equals item.Id
                join list in _context.ShoppingLists.AsNoTracking()
                    on item.ShoppingListId equals list.Id
                join supplier in _context.Suppliers.AsNoTracking()
                    on quote.SupplierId equals supplier.Id
                where list.UserId == userId
                orderby quote.CreatedAt descending
                select new UserItemQuoteDto(
                    quote.Id,
                    list.Id,
                    list.Name,
                    item.Id,
                    item.Name,
                    item.Quantity,
                    item.Unit,
                    supplier.Id,
                    supplier.Name,
                    quote.UnitPrice,
                    quote.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}
