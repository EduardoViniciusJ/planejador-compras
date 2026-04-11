namespace PlanejadorCompras.Domain.Repositories;

public interface IUnitOfWork : IDisposable
{
    Task CommitAsync(CancellationToken cancellationToken = default);
}
