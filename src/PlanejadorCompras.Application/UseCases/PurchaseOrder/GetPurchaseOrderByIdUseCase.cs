using PlanejadorCompras.Application.Features.PurchaseOrders.Contracts;
using PlanejadorCompras.Application.Services.Interfaces;

namespace PlanejadorCompras.Application.UseCases.PurchaseOrder;

public sealed class GetPurchaseOrderByIdUseCase(
    IPurchaseOrderAccessService purchaseOrderAccessService)
{
    public async Task<PurchaseOrderDetailResponseDto> ExecuteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var order = await purchaseOrderAccessService.GetForCurrentUserAsync(
            id,
            cancellationToken);

        return PurchaseOrderResponseMapper.ToDetail(order);
    }
}
