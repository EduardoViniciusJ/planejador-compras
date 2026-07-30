using PlanejadorCompras.Domain.Entities;

namespace PlanejadorCompras.Application.Services.Interfaces;

public interface ISavedEqualizationAccessService
{
    Task<SavedEqualization> GetForCurrentUserAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
