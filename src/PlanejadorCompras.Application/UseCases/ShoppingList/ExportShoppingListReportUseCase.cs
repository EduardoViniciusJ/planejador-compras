using PlanejadorCompras.Application.Features.Reports.Contracts;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Application.UseCases.Interfaces;

namespace PlanejadorCompras.Application.UseCases.ShoppingList;

public sealed class ExportShoppingListReportUseCase(
    IGetShoppingListReportDataUseCase getReportDataUseCase,
    IShoppingListPdfExporter pdfExporter,
    IShoppingListExcelExporter excelExporter) : IExportShoppingListReportUseCase
{
    public async Task<ExportedFileDto> ExportPdfAsync(
        Guid shoppingListId,
        CancellationToken cancellationToken = default)
    {
        var reportData = await getReportDataUseCase.ExecuteAsync(
            shoppingListId,
            cancellationToken);

        return await pdfExporter.ExportAsync(reportData, cancellationToken);
    }

    public async Task<ExportedFileDto> ExportExcelAsync(
        Guid shoppingListId,
        CancellationToken cancellationToken = default)
    {
        var reportData = await getReportDataUseCase.ExecuteAsync(
            shoppingListId,
            cancellationToken);

        return await excelExporter.ExportAsync(reportData, cancellationToken);
    }
}
