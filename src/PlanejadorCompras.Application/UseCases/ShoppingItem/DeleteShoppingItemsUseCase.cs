using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories;
using PlanejadorCompras.Domain.Repositories.ShoppingItem;

namespace PlanejadorCompras.Application.UseCases.ShoppingItem;

public sealed class DeleteShoppingItemsUseCase(IShoppingItemRepository repository, IUnitOfWork unitOfWork, IShoppingListAccessService access)
{
    public async Task ExecuteAsync(Guid listId, IReadOnlyList<Guid> ids, CancellationToken cancellationToken = default)
    {
        if (ids.Count is < 1 or > 200 || ids.Any(id => id == Guid.Empty))
            throw new BadRequestException("Select 1 to 200 items.", "invalid_item_batch");
        await access.GetForCurrentUserAsync(listId, cancellationToken);
        var uniqueIds = ids.Distinct().ToArray();
        foreach (var id in uniqueIds)
        {
            var item = await repository.GetByIdAsync(id, cancellationToken);
            if (item is null || item.ShoppingListId != listId)
                throw new NotFoundException("Shopping item not found.", "shopping_item_not_found");
        }
        foreach (var id in uniqueIds)
            if (!await repository.DeleteAsync(id, cancellationToken))
                throw new NotFoundException("Shopping item not found.", "shopping_item_not_found");
        await unitOfWork.CommitAsync(cancellationToken);
    }
}
