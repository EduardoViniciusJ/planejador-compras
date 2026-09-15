using PlanejadorCompras.Application.Features.ShoppingItems.Contracts;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Domain.Repositories.ShoppingItem;
using PlanejadorCompras.Domain.Repositories;
using ShoppingItemEntity = PlanejadorCompras.Domain.Entities.ShoppingItem;

namespace PlanejadorCompras.Application.UseCases.ShoppingItem;

public sealed class CreateShoppingItemsUseCase(IShoppingItemRepository repository, IUnitOfWork unitOfWork, IShoppingListAccessService access)
{
    public async Task<IReadOnlyList<Guid>> ExecuteAsync(Guid shoppingListId, IReadOnlyList<ShoppingItemRequestDto> requests, CancellationToken cancellationToken = default)
    {
        if (requests.Count is < 1 or > 200 || requests.Any(item => item.ShoppingListId != shoppingListId))
            throw new BadRequestException("Provide 1 to 200 items for the same shopping list.", "invalid_item_batch");
        await access.GetForCurrentUserAsync(shoppingListId, cancellationToken);
        // Validate the entire batch before adding entities; commit all items together.
        var items = requests.Select(item => ShoppingItemEntity.Create(shoppingListId, item.Name, item.Quantity, item.Unit)).ToArray();
        foreach (var item in items) await repository.AddAsync(item, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
        return items.Select(item => item.Id).ToArray();
    }
}
