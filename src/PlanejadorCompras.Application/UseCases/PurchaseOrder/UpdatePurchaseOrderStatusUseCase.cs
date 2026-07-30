using PlanejadorCompras.Application.Common.Dtos.Requests;
using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories;

namespace PlanejadorCompras.Application.UseCases.PurchaseOrder;

public sealed class UpdatePurchaseOrderStatusUseCase(
    IPurchaseOrderAccessService purchaseOrderAccessService,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
{
    public async Task<PurchaseOrderDetailResponseDto> ExecuteAsync(
        Guid id,
        UpdatePurchaseOrderStatusRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Status);

        var order = await purchaseOrderAccessService.GetForCurrentUserAsync(
            id,
            cancellationToken);
        var nowUtc = timeProvider.GetUtcNow().UtcDateTime;

        try
        {
            switch (request.Status.Trim().ToLowerInvariant())
            {
                case "completed":
                    order.Complete(nowUtc);
                    break;
                case "cancelled":
                    order.Cancel(nowUtc);
                    break;
                default:
                    throw new BadRequestException(
                        "Situacao de pedido invalida.",
                        "purchase_order_invalid_status");
            }
        }
        catch (InvalidOperationException exception)
        {
            throw new ConflictException(
                "Este pedido nao permite mais alteracao de situacao.",
                "purchase_order_status_transition_not_allowed",
                exception);
        }

        await unitOfWork.CommitAsync(cancellationToken);
        return PurchaseOrderResponseMapper.ToDetail(order);
    }
}
