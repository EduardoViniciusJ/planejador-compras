using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanejadorCompras.Application.Common.Dtos.Requests;
using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Application.UseCases.Interfaces;
using PlanejadorCompras.Application.UseCases.ShoppingList;
using PlanejadorCompras.Application.UseCases.ShoppingList.Create;

namespace PlanejadorCompras.API.Controllers;

[Authorize]
[ApiController]
[Route("api/shopping-lists")]
public sealed class ShoppingListsController : ControllerBase
{
    private readonly CreateShoppingListUseCase _createUseCase;
    private readonly GetShoppingListByIdUseCase _getByIdUseCase;
    private readonly GetShoppingListDetailUseCase _getDetailUseCase;
    private readonly GetShoppingListsByUserIdUseCase _getByUserIdUseCase;
    private readonly UpdateShoppingListUseCase _updateUseCase;
    private readonly DeleteShoppingListUseCase _deleteUseCase;
    private readonly ICalculateBestSupplierBudgetUseCase _calculateBestSupplierBudgetUseCase;
    private readonly IGetShoppingListEqualizationUseCase _getShoppingListEqualizationUseCase;
    private readonly GetShoppingListSuppliersUseCase _getSuppliersUseCase;
    private readonly AddSupplierToShoppingListUseCase _addSupplierUseCase;
    private readonly RemoveSupplierFromShoppingListUseCase _removeSupplierUseCase;

    public ShoppingListsController(
        CreateShoppingListUseCase createUseCase,
        GetShoppingListByIdUseCase getByIdUseCase,
        GetShoppingListDetailUseCase getDetailUseCase,
        GetShoppingListsByUserIdUseCase getByUserIdUseCase,
        UpdateShoppingListUseCase updateUseCase,
        DeleteShoppingListUseCase deleteUseCase,
        ICalculateBestSupplierBudgetUseCase calculateBestSupplierBudgetUseCase,
        IGetShoppingListEqualizationUseCase getShoppingListEqualizationUseCase,
        GetShoppingListSuppliersUseCase getSuppliersUseCase,
        AddSupplierToShoppingListUseCase addSupplierUseCase,
        RemoveSupplierFromShoppingListUseCase removeSupplierUseCase)
    {
        _createUseCase = createUseCase;
        _getByIdUseCase = getByIdUseCase;
        _getDetailUseCase = getDetailUseCase;
        _getByUserIdUseCase = getByUserIdUseCase;
        _updateUseCase = updateUseCase;
        _deleteUseCase = deleteUseCase;
        _calculateBestSupplierBudgetUseCase = calculateBestSupplierBudgetUseCase;
        _getShoppingListEqualizationUseCase = getShoppingListEqualizationUseCase;
        _getSuppliersUseCase = getSuppliersUseCase;
        _addSupplierUseCase = addSupplierUseCase;
        _removeSupplierUseCase = removeSupplierUseCase;
    }

    [HttpGet("{id:guid}/detail")]
    [ProducesResponseType(typeof(ShoppingListDetailResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDetail(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _getDetailUseCase.ExecuteAsync(id, cancellationToken);
        return Ok(result);
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
    [ProducesResponseType(typeof(ShoppingListsOverviewResponseDto), StatusCodes.Status200OK)]
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

    [HttpGet("{id:guid}/equalization")]
    [ProducesResponseType(typeof(EqualizationResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEqualization(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _getShoppingListEqualizationUseCase.ExecuteAsync(id, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}/suppliers")]
    [ProducesResponseType(typeof(IReadOnlyCollection<SupplierResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSuppliers(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _getSuppliersUseCase.ExecuteAsync(id, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id:guid}/suppliers/{supplierId:guid}")]
    [ProducesResponseType(typeof(SupplierResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddSupplier(
        Guid id,
        Guid supplierId,
        CancellationToken cancellationToken)
    {
        var result = await _addSupplierUseCase.ExecuteAsync(id, supplierId, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}/suppliers/{supplierId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveSupplier(
        Guid id,
        Guid supplierId,
        CancellationToken cancellationToken)
    {
        await _removeSupplierUseCase.ExecuteAsync(id, supplierId, cancellationToken);
        return NoContent();
    }
}
