using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories;
using PlanejadorCompras.Domain.Repositories.PurchaseOrder;

namespace PlanejadorCompras.Application.UseCases.PurchaseOrder;

public sealed class DeletePurchaseOrderUseCase(
    IPurchaseOrderRepository purchaseOrderRepository,
    IPurchaseOrderAccessService purchaseOrderAccessService,
    IUnitOfWork unitOfWork)
{
    public async Task ExecuteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await purchaseOrderAccessService.GetForCurrentUserAsync(id, cancellationToken);

        if (!await purchaseOrderRepository.DeleteAsync(id, cancellationToken))
        {
            throw new NotFoundException(
                "Purchase order not found.",
                "purchase_order_not_found");
        }

        await unitOfWork.CommitAsync(cancellationToken);
    }
}
