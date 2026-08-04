using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories;
using PlanejadorCompras.Domain.Repositories.Equalization;

namespace PlanejadorCompras.Application.UseCases.Equalization;

public sealed class DeleteSavedEqualizationUseCase(
    ISavedEqualizationRepository repository,
    ISavedEqualizationAccessService accessService,
    IUnitOfWork unitOfWork)
{
    public async Task ExecuteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await accessService.GetForCurrentUserAsync(id, cancellationToken);

        if (!await repository.DeleteAsync(id, cancellationToken))
        {
            throw new NotFoundException(
                "Equalization not found.",
                "equalization_not_found");
        }

        await unitOfWork.CommitAsync(cancellationToken);
    }
}
