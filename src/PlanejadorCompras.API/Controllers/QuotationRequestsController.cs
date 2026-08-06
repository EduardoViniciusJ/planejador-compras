using PlanejadorCompras.Application.Features.QuotationRequests.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanejadorCompras.Application.UseCases.QuotationRequest;

namespace PlanejadorCompras.API.Controllers;

[Authorize]
[ApiController]
[Route("api/quotation-requests")]
public sealed class QuotationRequestsController(
    CreateQuotationRequestUseCase createUseCase,
    GetQuotationRequestsUseCase getAllUseCase,
    GetQuotationRequestByIdUseCase getByIdUseCase,
    ExportSavedQuotationRequestPdfUseCase exportPdfUseCase)
    : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyCollection<QuotationRequestSummaryResponseDto>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await getAllUseCase.ExecuteAsync(cancellationToken));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(QuotationRequestDetailResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken) =>
        Ok(await getByIdUseCase.ExecuteAsync(id, cancellationToken));

    [HttpPost("/api/shopping-lists/{shoppingListId:guid}/quotation-requests")]
    [ProducesResponseType(typeof(QuotationRequestDetailResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(
        Guid shoppingListId,
        [FromBody] QuotationRequestPdfRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await createUseCase.ExecuteAsync(
            shoppingListId,
            request,
            cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
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
