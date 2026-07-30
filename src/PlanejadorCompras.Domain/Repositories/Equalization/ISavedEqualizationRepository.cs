using PlanejadorCompras.Domain.Entities;

namespace PlanejadorCompras.Domain.Repositories.Equalization;

public interface ISavedEqualizationRepository
{
    Task<SavedEqualization?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<SavedEqualization?> GetByRequestIdAsync(
        Guid userId,
        Guid requestId,
        CancellationToken cancellationToken = default);

    Task<(List<SavedEqualization> Items, int TotalCount)> SearchByUserIdAsync(
        Guid userId,
        string? searchTerm,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<bool> CodeExistsAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        SavedEqualization equalization,
        CancellationToken cancellationToken = default);
}
