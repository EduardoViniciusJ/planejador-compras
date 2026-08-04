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
        var input = SupplierRequestNormalizer.Normalize(request);
        var supplier = await supplierAccessService.GetForCurrentUserAsync(id, cancellationToken);

        if (await supplierRepository.ExistsByNameAsync(
                supplier.UserId,
                input.Name,
                supplier.Id,
                cancellationToken))
        {
            throw new ConflictException("Supplier name already exists.", "supplier_name_already_exists");
        }

        if (input.Cnpj is not null
            && await supplierRepository.ExistsByCnpjAsync(
                supplier.UserId,
                input.Cnpj,
                supplier.Id,
                cancellationToken))
        {
            throw new ConflictException(
                "Supplier CNPJ already exists.",
                "supplier_cnpj_already_exists");
        }

        supplier.Update(input.Name, input.Cnpj, input.Address, input.Contact);
        await supplierRepository.UpdateAsync(supplier, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return SupplierResponseMapper.Map(supplier);
    }
}
