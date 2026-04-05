using Microsoft.EntityFrameworkCore;
using PlanejadorCompras.Domain.Entities;
using PlanejadorCompras.Domain.Repositories.User;
using PlanejadorCompras.Infrastructure.Persistence;

namespace PlanejadorCompras.Infrastructure.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly PlanejadorComprasDbContext _context;

    public UserRepository(PlanejadorComprasDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
    }

    public async Task<User?> GetByGoogleIdAsync(string googleId, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.GoogleId == googleId, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
