using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanejadorCompras.Application.UseCases.Interfaces;

namespace PlanejadorCompras.API.Controllers;

[Authorize]
[ApiController]
[Route("api/shopping-lists/{id:guid}/reports")]
public sealed class ShoppingListReportsController(
    IExportShoppingListReportUseCase exportReportUseCase) : ControllerBase
{
    [HttpGet("pdf")]
    [Produces("application/pdf")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPdf(
        Guid id,
        CancellationToken cancellationToken)
    {
        var file = await exportReportUseCase.ExportPdfAsync(id, cancellationToken);
        return File(file.Content, file.ContentType, file.FileName);
    }

    [HttpGet("excel")]
    [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetExcel(
        Guid id,
        CancellationToken cancellationToken)
    {
        var file = await exportReportUseCase.ExportExcelAsync(id, cancellationToken);
        return File(file.Content, file.ContentType, file.FileName);
    }
}
