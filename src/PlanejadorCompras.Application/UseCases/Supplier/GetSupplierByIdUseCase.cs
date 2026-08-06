using PlanejadorCompras.Application.Features.Suppliers.Contracts;
using PlanejadorCompras.Application.Services.Interfaces;

namespace PlanejadorCompras.Application.UseCases.Supplier;

public sealed class GetSupplierByIdUseCase(ISupplierAccessService supplierAccessService)
{
    public async Task<SupplierResponseDto> ExecuteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var supplier = await supplierAccessService.GetForCurrentUserAsync(id, cancellationToken);
        return SupplierResponseMapper.Map(supplier);
    }
}
