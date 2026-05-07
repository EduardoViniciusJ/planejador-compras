using PlanejadorCompras.Application.Common.Dtos.Requests;
using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Domain.Repositories.ShoppingItem;
using PlanejadorCompras.Domain.Repositories;
using ShoppingItemEntity = PlanejadorCompras.Domain.Entities.ShoppingItem;

namespace PlanejadorCompras.Application.UseCases.ShoppingItem.Create;

public sealed class CreateShoppingItemUseCase
{
    private readonly IShoppingItemRepository _shoppingItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateShoppingItemUseCase(
        IShoppingItemRepository shoppingItemRepository,
        IUnitOfWork unitOfWork)
    {
        _shoppingItemRepository = shoppingItemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ShoppingItemResponseDto> ExecuteAsync(
        ShoppingItemRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var shoppingItem = ShoppingItemEntity.Create(request.ShoppingListId, request.Name, request.Quantity, request.Unit);
        await _shoppingItemRepository.AddAsync(shoppingItem, cancellationToken);
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
