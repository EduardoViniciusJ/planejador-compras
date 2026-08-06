using PlanejadorCompras.Application.Features.ShoppingItems.Contracts;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories.ShoppingItem;
using PlanejadorCompras.Domain.Repositories;
using ShoppingItemEntity = PlanejadorCompras.Domain.Entities.ShoppingItem;

namespace PlanejadorCompras.Application.UseCases.ShoppingItem.Create;

public sealed class CreateShoppingItemUseCase
{
    private readonly IShoppingItemRepository _shoppingItemRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IShoppingListAccessService _shoppingListAccessService;

    public CreateShoppingItemUseCase(
        IShoppingItemRepository shoppingItemRepository,
        IUnitOfWork unitOfWork,
        IShoppingListAccessService shoppingListAccessService)
    {
        _shoppingItemRepository = shoppingItemRepository;
        _unitOfWork = unitOfWork;
        _shoppingListAccessService = shoppingListAccessService;
    }

    public async Task<ShoppingItemResponseDto> ExecuteAsync(
        ShoppingItemRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        await _shoppingListAccessService.GetForCurrentUserAsync(request.ShoppingListId, cancellationToken);

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
