using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanejadorCompras.Application.Common.Dtos.Requests;
using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Application.UseCases.ShoppingList;
using PlanejadorCompras.Application.UseCases.ShoppingList.Create;

namespace PlanejadorCompras.API.Controllers;

[ApiController]
[Route("api/shopping-lists")]
[Authorize]
public sealed class ShoppingListsController : ControllerBase
{
    private readonly CreateShoppingListUseCase _createUseCase;
    private readonly GetShoppingListByIdUseCase _getByIdUseCase;
    private readonly GetShoppingListsByUserIdUseCase _getByUserIdUseCase;
    private readonly UpdateShoppingListUseCase _updateUseCase;
    private readonly DeleteShoppingListUseCase _deleteUseCase;
    private readonly ICalculateBestSupplierBudgetUseCase _calculateBestSupplierBudgetUseCase;

    public ShoppingListsController(
        CreateShoppingListUseCase createUseCase,
        GetShoppingListByIdUseCase getByIdUseCase,
        GetShoppingListsByUserIdUseCase getByUserIdUseCase,
        UpdateShoppingListUseCase updateUseCase,
        DeleteShoppingListUseCase deleteUseCase,
        ICalculateBestSupplierBudgetUseCase calculateBestSupplierBudgetUseCase)
    {
        _createUseCase = createUseCase;
        _getByIdUseCase = getByIdUseCase;
        _getByUserIdUseCase = getByUserIdUseCase;
        _updateUseCase = updateUseCase;
        _deleteUseCase = deleteUseCase;
        _calculateBestSupplierBudgetUseCase = calculateBestSupplierBudgetUseCase;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ShoppingListResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] ShoppingListRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _createUseCase.ExecuteAsync(request, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ShoppingListResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _getByIdUseCase.ExecuteAsync(id, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ShoppingListResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByUserId(CancellationToken cancellationToken)
    {
        var result = await _getByUserIdUseCase.ExecuteAsync(cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ShoppingListResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] ShoppingListRequestDto request,
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

    [HttpGet("{id:guid}/best-supplier-budget")]
    [ProducesResponseType(typeof(BestSupplierBudgetResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBestSupplierBudget(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _calculateBestSupplierBudgetUseCase.ExecuteAsync(id, cancellationToken);
        return Ok(result);
    }
}
