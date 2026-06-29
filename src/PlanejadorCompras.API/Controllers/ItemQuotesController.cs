using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanejadorCompras.Application.Common.Dtos.Requests;
using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Application.UseCases.ItemQuote;
using PlanejadorCompras.Application.UseCases.ItemQuote.Create;

namespace PlanejadorCompras.API.Controllers;

[ApiController]
[Route("api/item-quotes")]
public sealed class ItemQuotesController : ControllerBase
{
    private readonly CreateItemQuoteUseCase _createUseCase;
    private readonly GetItemQuoteByIdUseCase _getByIdUseCase;
    private readonly GetItemQuotesByShoppingItemIdUseCase _getByShoppingItemIdUseCase;
    private readonly UpdateItemQuoteUseCase _updateUseCase;
    private readonly DeleteItemQuoteUseCase _deleteUseCase;

    public ItemQuotesController(
        CreateItemQuoteUseCase createUseCase,
        GetItemQuoteByIdUseCase getByIdUseCase,
        GetItemQuotesByShoppingItemIdUseCase getByShoppingItemIdUseCase,
        UpdateItemQuoteUseCase updateUseCase,
        DeleteItemQuoteUseCase deleteUseCase)
    {
        _createUseCase = createUseCase;
        _getByIdUseCase = getByIdUseCase;
        _getByShoppingItemIdUseCase = getByShoppingItemIdUseCase;
        _updateUseCase = updateUseCase;
        _deleteUseCase = deleteUseCase;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ItemQuoteResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] ItemQuoteRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _createUseCase.ExecuteAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ItemQuoteResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _getByIdUseCase.ExecuteAsync(id, cancellationToken);
        return Ok(result);
    }

    [HttpGet("/api/shopping-items/{shoppingItemId:guid}/quotes")]
    [ProducesResponseType(typeof(List<ItemQuoteResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByShoppingItemId(
        Guid shoppingItemId,
        CancellationToken cancellationToken)
    {
        var result = await _getByShoppingItemIdUseCase.ExecuteAsync(shoppingItemId, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ItemQuoteResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] ItemQuoteRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _updateUseCase.ExecuteAsync(id, request, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _deleteUseCase.ExecuteAsync(id, cancellationToken);
        return NoContent();
    }
}
