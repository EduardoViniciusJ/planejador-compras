using ClosedXML.Excel;
using PlanejadorCompras.Application.Features.Reports.Contracts;
using PlanejadorCompras.Application.Services.Interfaces;

namespace PlanejadorCompras.Infrastructure.Reports.Excel;

public sealed class ClosedXmlShoppingListReportExporter : IShoppingListExcelExporter
{
    private const string ContentType =
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    public Task<ExportedFileDto> ExportAsync(
        ShoppingListReportDataDto reportData,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(reportData);
        cancellationToken.ThrowIfCancellationRequested();

        using var workbook = new XLWorkbook();

        ShoppingListSummaryWorksheetBuilder.Build(workbook, reportData);
        cancellationToken.ThrowIfCancellationRequested();

        ShoppingListPriceMapWorksheetBuilder.Build(workbook, reportData);
        cancellationToken.ThrowIfCancellationRequested();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(new ExportedFileDto(
            stream.ToArray(),
            ContentType,
            ReportFileNameBuilder.BuildEqualizationFileName(
                reportData.Name,
                "xlsx")));
    }
}
