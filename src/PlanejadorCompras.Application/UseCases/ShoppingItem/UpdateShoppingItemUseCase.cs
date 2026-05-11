using PlanejadorCompras.Application.Common.Dtos.Requests;
using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Domain.Repositories.ShoppingItem;
using PlanejadorCompras.Domain.Repositories;

namespace PlanejadorCompras.Application.UseCases.ShoppingItem;

public sealed class UpdateShoppingItemUseCase
{
    private readonly IShoppingItemRepository _shoppingItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateShoppingItemUseCase(
        IShoppingItemRepository shoppingItemRepository,
        IUnitOfWork unitOfWork)
    {
        _shoppingItemRepository = shoppingItemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ShoppingItemResponseDto> ExecuteAsync(
        Guid id,
        ShoppingItemRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentOutOfRangeException.ThrowIfEqual(id, Guid.Empty);

        var shoppingItem = await _shoppingItemRepository.GetByIdAsync(id, cancellationToken);
        if (shoppingItem is null)
        {
            throw new NotFoundException("Shopping item not found.", "shopping_item_not_found");
        }

        shoppingItem.Update(request.ShoppingListId, request.Name, request.Quantity, request.Unit);
        await _shoppingItemRepository.UpdateAsync(shoppingItem, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return new ShoppingItemResponseDto(
            shoppingItem.Id,
            shoppingItem.ShoppingListId,
            shoppingItem.Name,
            shoppingItem.Quantity,
            shoppingItem.Unit,
            shoppingItem.CreatedAt);
    }
}
