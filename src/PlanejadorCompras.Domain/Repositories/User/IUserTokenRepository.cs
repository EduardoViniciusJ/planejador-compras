namespace PlanejadorCompras.Domain.Repositories.User;

public interface IUserTokenRepository
{
    Task AddAsync(Entities.UserToken userToken, CancellationToken cancellationToken = default);
    Task<Entities.UserToken?> GetByTokenAsync(string token, string type, CancellationToken cancellationToken = default);
    Task RemoveByUserAndTypeAsync(Guid userId, string type, CancellationToken cancellationToken = default);
    void Remove(Entities.UserToken userToken);
}
