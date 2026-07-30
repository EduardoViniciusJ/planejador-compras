using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories.PurchaseOrder;
using PurchaseOrderEntity = PlanejadorCompras.Domain.Entities.PurchaseOrder;

namespace PlanejadorCompras.Infrastructure.Services;

public sealed class PurchaseOrderAccessService(
    IPurchaseOrderRepository purchaseOrderRepository,
    ICurrentUser currentUser)
    : IPurchaseOrderAccessService
{
    public async Task<PurchaseOrderEntity> GetForCurrentUserAsync(
        Guid purchaseOrderId,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(purchaseOrderId, Guid.Empty);

        var order = await purchaseOrderRepository.GetByIdAsync(
            purchaseOrderId,
            cancellationToken);

        if (order is null || order.UserId != currentUser.UserId)
        {
            throw new NotFoundException(
                "Purchase order not found.",
                "purchase_order_not_found");
        }

        return order;
    }
}
