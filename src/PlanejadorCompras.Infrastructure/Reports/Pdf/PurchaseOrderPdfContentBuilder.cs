using PlanejadorCompras.Application.Features.Reports.Contracts;
using MigraDoc.DocumentObjectModel;

namespace PlanejadorCompras.Infrastructure.Reports.Pdf;

internal static class PurchaseOrderPdfContentBuilder
{
    internal static void AddContent(Section section, PurchaseOrderReportDataDto data)
    {
        AddTitle(section, data);
        PurchaseOrderPdfInformationBuilder.Add(section, data);
        PurchaseOrderPdfItemsBuilder.Add(section, data);
        AddAdditionalInformation(section, data);
    }

    private static void AddTitle(Section section, PurchaseOrderReportDataDto data)
    {
        var title = section.AddParagraph("Pedido de compra");
        title.Format.Font.Size = Unit.FromPoint(20);
        title.Format.Font.Bold = true;
        title.Format.SpaceAfter = Unit.FromPoint(3);

        var identification = section.AddParagraph();
        identification.Format.Font.Size = Unit.FromPoint(10);
        identification.Format.Font.Color = OperationalPdfTheme.MutedColor;
        identification.Format.SpaceAfter = Unit.FromPoint(10);
        identification.AddFormattedText(data.Code, TextFormat.Bold);
        identification.AddText(
            $" | Emitido em {data.CreatedAtUtc.ToString("dd/MM/yyyy", OperationalPdfTheme.BrazilianCulture)}"
            + $" | Situação: {data.Status}");
    }

    private static void AddAdditionalInformation(
        Section section,
        PurchaseOrderReportDataDto data)
    {
        if (!string.IsNullOrWhiteSpace(data.DeliveryAddress))
        {
            AddTextSection(section, "Local de entrega", data.DeliveryAddress);
        }

        if (!string.IsNullOrWhiteSpace(data.Notes))
        {
            AddTextSection(section, "Observações", data.Notes);
        }
    }

    private static void AddTextSection(Section section, string title, string value)
    {
        section.AddParagraph(title, StyleNames.Heading1);
        var paragraph = section.AddParagraph(value);
        paragraph.Format.Borders.Width = Unit.FromPoint(0.5);
        paragraph.Format.Borders.Color = OperationalPdfTheme.BorderColor;
        paragraph.Format.LeftIndent = Unit.FromPoint(6);
        paragraph.Format.RightIndent = Unit.FromPoint(6);
        paragraph.Format.SpaceBefore = Unit.FromPoint(2);
        paragraph.Format.SpaceAfter = Unit.FromPoint(4);
    }

}
