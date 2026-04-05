namespace PlanejadorCompras.Domain.Repositories.User;

public interface IUserRepository
{
    Task<Entities.User?> GetByGoogleIdAsync(string googleId, CancellationToken cancellationToken = default);
    Task AddAsync(Entities.User user, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
