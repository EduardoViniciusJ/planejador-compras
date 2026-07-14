using PlanejadorCompras.Application.Common.Dtos.Requests;
using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories;
using PlanejadorCompras.Domain.Repositories.Supplier;
using SupplierEntity = PlanejadorCompras.Domain.Entities.Supplier;

namespace PlanejadorCompras.Application.UseCases.Supplier;

public sealed class CreateSupplierUseCase(
    ISupplierRepository supplierRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser)
{
    public async Task<SupplierResponseDto> ExecuteAsync(
        SupplierRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (await supplierRepository.ExistsByNameAsync(
                currentUser.UserId,
                request.Name,
                cancellationToken: cancellationToken))
        {
            throw new ConflictException("Supplier name already exists.", "supplier_name_already_exists");
        }

        var supplier = SupplierEntity.Create(currentUser.UserId, request.Name);
        await supplierRepository.AddAsync(supplier, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return new SupplierResponseDto(supplier.Id, supplier.Name, supplier.CreatedAt);
    }
}
