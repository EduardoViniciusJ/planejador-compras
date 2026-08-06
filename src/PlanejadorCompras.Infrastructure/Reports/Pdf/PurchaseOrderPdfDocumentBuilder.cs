using PlanejadorCompras.Application.Features.Reports.Contracts;
using MigraDoc.DocumentObjectModel;

namespace PlanejadorCompras.Infrastructure.Reports.Pdf;

public sealed class PurchaseOrderPdfDocumentBuilder
{
    public Document Build(PurchaseOrderReportDataDto data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var document = OperationalPdfTheme.CreateDocument(
            $"Pedido de compra {data.Code}",
            "Pedido de compra");
        var section = OperationalPdfTheme.AddPortraitSection(document, 1.5);
        OperationalPdfTheme.AddFooter(section, 8);
        PurchaseOrderPdfContentBuilder.AddContent(section, data);

        return document;
    }
}
