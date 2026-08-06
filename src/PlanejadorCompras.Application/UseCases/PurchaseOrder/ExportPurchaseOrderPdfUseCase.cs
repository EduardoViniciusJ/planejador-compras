using PlanejadorCompras.Application.Features.Reports.Contracts;
using PlanejadorCompras.Application.Services.Interfaces;

namespace PlanejadorCompras.Application.UseCases.PurchaseOrder;

public sealed class ExportPurchaseOrderPdfUseCase(
    IPurchaseOrderAccessService purchaseOrderAccessService,
    IPurchaseOrderPdfExporter pdfExporter)
{
    public async Task<ExportedFileDto> ExecuteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var order = await purchaseOrderAccessService.GetForCurrentUserAsync(
            id,
            cancellationToken);
        var report = new PurchaseOrderReportDataDto(
            order.Code,
            order.ShoppingListName,
            order.SupplierName,
            order.BuyerName,
            order.BuyerEmail,
            order.ExpectedDeliveryDate,
            order.DeliveryAddress,
            order.PaymentTerms,
            order.Notes,
            PurchaseOrderResponseMapper.ToDisplayStatus(order.Status),
            order.CreatedAtUtc,
            order.TotalPrice,
            order.Items
                .Select(item => new PurchaseOrderReportItemDto(
                    item.Name,
                    item.Quantity,
                    item.Unit,
                    item.UnitPrice,
                    item.TotalPrice))
                .ToList());

        return await pdfExporter.ExportAsync(report, cancellationToken);
    }
}
