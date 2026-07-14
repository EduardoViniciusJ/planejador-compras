using PlanejadorCompras.Application.Common.Dtos.Requests;
using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories;
using PlanejadorCompras.Domain.Repositories.Supplier;

namespace PlanejadorCompras.Application.UseCases.Supplier;

public sealed class UpdateSupplierUseCase(
    ISupplierRepository supplierRepository,
    IUnitOfWork unitOfWork,
    ISupplierAccessService supplierAccessService)
{
    public async Task<SupplierResponseDto> ExecuteAsync(
        Guid id,
        SupplierRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var supplier = await supplierAccessService.GetForCurrentUserAsync(id, cancellationToken);

        if (await supplierRepository.ExistsByNameAsync(
                supplier.UserId,
                request.Name,
                supplier.Id,
                cancellationToken))
        {
            throw new ConflictException("Supplier name already exists.", "supplier_name_already_exists");
        }

        supplier.Update(request.Name);
        await supplierRepository.UpdateAsync(supplier, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return new SupplierResponseDto(supplier.Id, supplier.Name, supplier.CreatedAt);
    }
}
