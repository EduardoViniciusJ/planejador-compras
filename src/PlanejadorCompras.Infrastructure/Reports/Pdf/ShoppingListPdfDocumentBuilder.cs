using MigraDoc.DocumentObjectModel;
using PlanejadorCompras.Application.Features.Reports.Contracts;

namespace PlanejadorCompras.Infrastructure.Reports.Pdf;

public sealed class ShoppingListPdfDocumentBuilder
{
    public Document Build(ShoppingListReportDataDto reportData)
    {
        ArgumentNullException.ThrowIfNull(reportData);

        var document = ShoppingListPdfDocumentFactory.Create(reportData);
        var supplierGroups = CreateSupplierGroups(reportData.Suppliers);

        for (var groupIndex = 0; groupIndex < supplierGroups.Count; groupIndex++)
        {
            var section = ShoppingListPdfDocumentFactory.AddSection(document);
            ShoppingListPdfPageChrome.AddHeader(section, reportData);
            ShoppingListPdfPageChrome.AddFooter(section);

            if (groupIndex == 0)
            {
                ShoppingListPdfPageChrome.AddIntroduction(section, reportData);
                ShoppingListPdfSummaryBuilder.Add(section, reportData);
            }

            ShoppingListPdfSummaryBuilder.AddPriceMapTitle(
                section,
                groupIndex,
                supplierGroups.Count,
                reportData.Suppliers.Count);
            ShoppingListPriceMapTableBuilder.Add(
                section,
                reportData,
                supplierGroups[groupIndex]);
        }

        return document;
    }

    private static IReadOnlyList<IReadOnlyList<ShoppingListReportSupplierDto>>
        CreateSupplierGroups(
            IReadOnlyCollection<ShoppingListReportSupplierDto> suppliers)
    {
        if (suppliers.Count == 0)
        {
            return new[] { Array.Empty<ShoppingListReportSupplierDto>() };
        }

        return suppliers
            .Chunk(ShoppingListPdfTheme.MaxSuppliersPerGroup)
            .Select(group => (IReadOnlyList<ShoppingListReportSupplierDto>)group)
            .ToList();
    }
}
