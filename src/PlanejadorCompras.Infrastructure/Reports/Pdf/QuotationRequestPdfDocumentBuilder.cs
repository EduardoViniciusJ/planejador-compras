using PlanejadorCompras.Application.Features.Reports.Contracts;
using MigraDoc.DocumentObjectModel;
using PlanejadorCompras.Infrastructure.Reports;

namespace PlanejadorCompras.Infrastructure.Reports.Pdf;

public sealed class QuotationRequestPdfDocumentBuilder
{
    public Document Build(QuotationRequestReportDataDto data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var document = OperationalPdfTheme.CreateDocument(
            $"Solicitação de cotação {data.Code}",
            "Solicitação de cotação");
        var section = OperationalPdfTheme.AddPortraitSection(document, 1.6);
        QuotationRequestPdfContentBuilder.AddContent(section, data);
        OperationalPdfTheme.AddFooter(section, ReportDesignSystem.FooterFontSize);

        return document;
    }
}
