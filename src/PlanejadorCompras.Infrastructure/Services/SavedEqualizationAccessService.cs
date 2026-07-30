using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Entities;
using PlanejadorCompras.Domain.Repositories.Equalization;

namespace PlanejadorCompras.Infrastructure.Services;

public sealed class SavedEqualizationAccessService(
    ISavedEqualizationRepository repository,
    ICurrentUser currentUser)
    : ISavedEqualizationAccessService
{
    public async Task<SavedEqualization> GetForCurrentUserAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(id, Guid.Empty);

        var equalization = await repository.GetByIdAsync(id, cancellationToken);

        if (equalization is null || equalization.UserId != currentUser.UserId)
        {
            throw new NotFoundException(
                "Equalization not found.",
                "equalization_not_found");
        }

        return equalization;
    }
}
