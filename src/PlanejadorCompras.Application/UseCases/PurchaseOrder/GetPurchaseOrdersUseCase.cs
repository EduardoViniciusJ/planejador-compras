using PlanejadorCompras.Application.Features.PurchaseOrders.Contracts;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories.PurchaseOrder;

namespace PlanejadorCompras.Application.UseCases.PurchaseOrder;

public sealed class GetPurchaseOrdersUseCase(
    IPurchaseOrderRepository purchaseOrderRepository,
    ICurrentUser currentUser)
{
    public async Task<IReadOnlyCollection<PurchaseOrderSummaryResponseDto>> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        var orders = await purchaseOrderRepository.GetByUserIdAsync(
            currentUser.UserId,
            cancellationToken);

        return orders
            .Select(PurchaseOrderResponseMapper.ToSummary)
            .ToList();
    }
}
