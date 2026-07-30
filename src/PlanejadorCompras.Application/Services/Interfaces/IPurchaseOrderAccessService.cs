using PurchaseOrderEntity = PlanejadorCompras.Domain.Entities.PurchaseOrder;

namespace PlanejadorCompras.Application.Services.Interfaces;

public interface IPurchaseOrderAccessService
{
    Task<PurchaseOrderEntity> GetForCurrentUserAsync(
        Guid purchaseOrderId,
        CancellationToken cancellationToken = default);
}
