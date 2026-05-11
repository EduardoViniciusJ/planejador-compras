using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Domain.Repositories.ShoppingItem;
using PlanejadorCompras.Domain.Repositories;

namespace PlanejadorCompras.Application.UseCases.ShoppingItem;

public sealed class DeleteShoppingItemUseCase
{
    private readonly IShoppingItemRepository _shoppingItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteShoppingItemUseCase(
        IShoppingItemRepository shoppingItemRepository,
        IUnitOfWork unitOfWork)
    {
        _shoppingItemRepository = shoppingItemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(id, Guid.Empty);

        var deleted = await _shoppingItemRepository.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            throw new NotFoundException("Shopping item not found.", "shopping_item_not_found");
        }

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
