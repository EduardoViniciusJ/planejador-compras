using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanejadorCompras.Application.Common.Dtos.Requests;
using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Application.UseCases.PurchaseOrder;

namespace PlanejadorCompras.API.Controllers;

[Authorize]
[ApiController]
[Route("api/purchase-orders")]
public sealed class PurchaseOrdersController(
    GetPurchaseOrderDraftUseCase getDraftUseCase,
    CreatePurchaseOrderUseCase createUseCase,
    GetPurchaseOrdersUseCase getAllUseCase,
    GetPurchaseOrderByIdUseCase getByIdUseCase,
    UpdatePurchaseOrderStatusUseCase updateStatusUseCase,
    DeletePurchaseOrderUseCase deleteUseCase,
    ExportPurchaseOrderPdfUseCase exportPdfUseCase)
    : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyCollection<PurchaseOrderSummaryResponseDto>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await getAllUseCase.ExecuteAsync(cancellationToken));

    [HttpGet("draft")]
    [ProducesResponseType(typeof(PurchaseOrderDraftResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDraft(
        [FromQuery] Guid shoppingListId,
        [FromQuery] Guid supplierId,
        [FromQuery] Guid? equalizationId,
        CancellationToken cancellationToken) =>
        Ok(await getDraftUseCase.ExecuteAsync(
            shoppingListId,
            supplierId,
            equalizationId,
            cancellationToken));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PurchaseOrderDetailResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken) =>
        Ok(await getByIdUseCase.ExecuteAsync(id, cancellationToken));

    [HttpPost]
    [ProducesResponseType(typeof(PurchaseOrderDetailResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreatePurchaseOrderRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await createUseCase.ExecuteAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(PurchaseOrderDetailResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        [FromBody] UpdatePurchaseOrderStatusRequestDto request,
        CancellationToken cancellationToken) =>
        Ok(await updateStatusUseCase.ExecuteAsync(id, request, cancellationToken));

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await deleteUseCase.ExecuteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpGet("{id:guid}/pdf")]
    [Produces("application/pdf")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPdf(
        Guid id,
        CancellationToken cancellationToken)
    {
        var file = await exportPdfUseCase.ExecuteAsync(id, cancellationToken);
        return File(file.Content, file.ContentType, file.FileName);
    }
}
