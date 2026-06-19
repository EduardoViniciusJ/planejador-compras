using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories.ShoppingItem;
using PlanejadorCompras.Domain.Repositories;

namespace PlanejadorCompras.Application.UseCases.ShoppingItem;

public sealed class DeleteShoppingItemUseCase
{
    private readonly IShoppingItemRepository _shoppingItemRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IShoppingListAccessService _shoppingListAccessService;

    public DeleteShoppingItemUseCase(
        IShoppingItemRepository shoppingItemRepository,
        IUnitOfWork unitOfWork,
        IShoppingListAccessService shoppingListAccessService)
    {
        _shoppingItemRepository = shoppingItemRepository;
        _unitOfWork = unitOfWork;
        _shoppingListAccessService = shoppingListAccessService;
    }

    public async Task ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(id, Guid.Empty);

        var shoppingItem = await _shoppingItemRepository.GetByIdAsync(id, cancellationToken);
        if (shoppingItem is null)
        {
            throw new NotFoundException("Shopping item not found.", "shopping_item_not_found");
        }

        await _shoppingListAccessService.GetForCurrentUserAsync(shoppingItem.ShoppingListId, cancellationToken);

        var deleted = await _shoppingItemRepository.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            throw new NotFoundException("Shopping item not found.", "shopping_item_not_found");
        }

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
