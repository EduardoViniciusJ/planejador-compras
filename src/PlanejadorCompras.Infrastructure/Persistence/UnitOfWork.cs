using PlanejadorCompras.Domain.Repositories;

namespace PlanejadorCompras.Infrastructure.Persistence;

public sealed class UnitOfWork(PlanejadorComprasDbContext dbContext) : IUnitOfWork
{
    private readonly PlanejadorComprasDbContext _dbContext = dbContext;

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
