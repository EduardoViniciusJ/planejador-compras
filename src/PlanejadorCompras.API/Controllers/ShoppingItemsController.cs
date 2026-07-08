using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanejadorCompras.Application.Common.Dtos.Requests;
using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Application.UseCases.ShoppingItem;
using PlanejadorCompras.Application.UseCases.ShoppingItem.Create;

namespace PlanejadorCompras.API.Controllers;

[Authorize]
[ApiController]
[Route("api/shopping-items")]
public sealed class ShoppingItemsController : ControllerBase
{
    private readonly CreateShoppingItemUseCase _createUseCase;
    private readonly GetShoppingItemByIdUseCase _getByIdUseCase;
    private readonly GetShoppingItemsByShoppingListIdUseCase _getByShoppingListIdUseCase;
    private readonly UpdateShoppingItemUseCase _updateUseCase;
    private readonly DeleteShoppingItemUseCase _deleteUseCase;

    public ShoppingItemsController(
        CreateShoppingItemUseCase createUseCase,
        GetShoppingItemByIdUseCase getByIdUseCase,
        GetShoppingItemsByShoppingListIdUseCase getByShoppingListIdUseCase,
        UpdateShoppingItemUseCase updateUseCase,
        DeleteShoppingItemUseCase deleteUseCase)
    {
        _createUseCase = createUseCase;
        _getByIdUseCase = getByIdUseCase;
        _getByShoppingListIdUseCase = getByShoppingListIdUseCase;
        _updateUseCase = updateUseCase;
        _deleteUseCase = deleteUseCase;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ShoppingItemResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] ShoppingItemRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _createUseCase.ExecuteAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ShoppingItemResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _getByIdUseCase.ExecuteAsync(id, cancellationToken);
        return Ok(result);
    }

    [HttpGet("/api/shopping-lists/{shoppingListId:guid}/items")]
    [ProducesResponseType(typeof(List<ShoppingItemResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByShoppingListId(
        Guid shoppingListId,
        CancellationToken cancellationToken)
    {
        var result = await _getByShoppingListIdUseCase.ExecuteAsync(shoppingListId, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ShoppingItemResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] ShoppingItemRequestDto request,
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
