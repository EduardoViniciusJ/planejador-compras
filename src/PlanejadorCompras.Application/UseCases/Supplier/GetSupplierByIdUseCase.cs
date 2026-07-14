using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Application.Services.Interfaces;

namespace PlanejadorCompras.Application.UseCases.Supplier;

public sealed class GetSupplierByIdUseCase(ISupplierAccessService supplierAccessService)
{
    public async Task<SupplierResponseDto> ExecuteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var supplier = await supplierAccessService.GetForCurrentUserAsync(id, cancellationToken);
        return new SupplierResponseDto(supplier.Id, supplier.Name, supplier.CreatedAt);
    }
}
