using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories.Supplier;

namespace PlanejadorCompras.Application.UseCases.Supplier;

public sealed class GetSuppliersUseCase(
    ISupplierRepository supplierRepository,
    ICurrentUser currentUser)
{
    public async Task<IReadOnlyCollection<SupplierResponseDto>> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        var suppliers = await supplierRepository.GetByUserIdAsync(currentUser.UserId, cancellationToken);
        return suppliers
            .Select(SupplierResponseMapper.Map)
            .ToList();
    }
}
