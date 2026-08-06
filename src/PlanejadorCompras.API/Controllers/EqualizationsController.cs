using PlanejadorCompras.Application.Features.Equalizations.Contracts;
using PlanejadorCompras.Application.Common.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanejadorCompras.Application.UseCases.Equalization;

namespace PlanejadorCompras.API.Controllers;

[Authorize]
[ApiController]
[Route("api/equalizations")]
public sealed class EqualizationsController(
    CreateSavedEqualizationUseCase createUseCase,
    GetSavedEqualizationsUseCase getAllUseCase,
    GetSavedEqualizationByIdUseCase getByIdUseCase,
    DeleteSavedEqualizationUseCase deleteUseCase)
    : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(
        typeof(PagedResponseDto<SavedEqualizationSummaryResponseDto>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12,
        CancellationToken cancellationToken = default) =>
        Ok(await getAllUseCase.ExecuteAsync(
            search,
            page,
            pageSize,
            cancellationToken));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(
        typeof(SavedEqualizationDetailResponseDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken) =>
        Ok(await getByIdUseCase.ExecuteAsync(id, cancellationToken));

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

    [HttpPost("/api/shopping-lists/{shoppingListId:guid}/equalizations")]
    [ProducesResponseType(
        typeof(SavedEqualizationDetailResponseDto),
        StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        Guid shoppingListId,
        [FromBody] CreateSavedEqualizationRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await createUseCase.ExecuteAsync(
            shoppingListId,
            request,
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
}
