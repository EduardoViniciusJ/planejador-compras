using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories;
using PlanejadorCompras.Domain.Repositories.Supplier;

namespace PlanejadorCompras.Application.UseCases.Supplier;

public sealed class DeleteSupplierUseCase(
    ISupplierRepository supplierRepository,
    IUnitOfWork unitOfWork,
    ISupplierAccessService supplierAccessService)
{
    public async Task ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await supplierAccessService.GetForCurrentUserAsync(id, cancellationToken);

        if (await supplierRepository.HasQuotesAsync(id, cancellationToken))
        {
            throw new ConflictException(
                "Supplier has item quotes and cannot be deleted.",
                "supplier_has_quotes");
        }

        if (!await supplierRepository.DeleteAsync(id, cancellationToken))
        {
            throw new NotFoundException("Supplier not found.", "supplier_not_found");
        }

        await unitOfWork.CommitAsync(cancellationToken);
    }
}
