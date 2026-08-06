using PlanejadorCompras.Application.Features.Suppliers.Contracts;
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
        var input = SupplierRequestNormalizer.Normalize(request);

        if (await supplierRepository.ExistsByNameAsync(
                currentUser.UserId,
                input.Name,
                cancellationToken: cancellationToken))
        {
            throw new ConflictException("Supplier name already exists.", "supplier_name_already_exists");
        }

        if (input.Cnpj is not null
            && await supplierRepository.ExistsByCnpjAsync(
                currentUser.UserId,
                input.Cnpj,
                cancellationToken: cancellationToken))
        {
            throw new ConflictException(
                "Supplier CNPJ already exists.",
                "supplier_cnpj_already_exists");
        }

        var supplier = SupplierEntity.Create(
            currentUser.UserId,
            input.Name,
            input.Cnpj,
            input.Address,
            input.Contact);
        await supplierRepository.AddAsync(supplier, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return SupplierResponseMapper.Map(supplier);
    }
}
