using Microsoft.EntityFrameworkCore;
using PlanejadorCompras.Domain.Entities;
using PlanejadorCompras.Domain.Repositories.User;
using PlanejadorCompras.Infrastructure.Persistence;

namespace PlanejadorCompras.Infrastructure.Repositories;

public sealed class UserTokenRepository : IUserTokenRepository
{
    private readonly PlanejadorComprasDbContext _context;

    public UserTokenRepository(PlanejadorComprasDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(UserToken userToken, CancellationToken cancellationToken = default)
    {
        await _context.UserTokens.AddAsync(userToken, cancellationToken);
    }

    public async Task<UserToken?> GetByTokenAsync(string token, string type, CancellationToken cancellationToken = default)
    {
        return await _context.UserTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Token == token && x.Type == type, cancellationToken);
    }

    public void Remove(UserToken userToken)
    {
        _context.UserTokens.Remove(userToken);
    }
}
