using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Domain.Repositories.ShoppingList;
using PlanejadorCompras.Domain.Repositories;

namespace PlanejadorCompras.Application.UseCases.ShoppingList;

public sealed class DeleteShoppingListUseCase
{
    private readonly IShoppingListRepository _shoppingListRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteShoppingListUseCase(
        IShoppingListRepository shoppingListRepository,
        IUnitOfWork unitOfWork)
    {
        _shoppingListRepository = shoppingListRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(id, Guid.Empty);

        var deleted = await _shoppingListRepository.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            throw new NotFoundException("Shopping list not found.", "shopping_list_not_found");
        }

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
