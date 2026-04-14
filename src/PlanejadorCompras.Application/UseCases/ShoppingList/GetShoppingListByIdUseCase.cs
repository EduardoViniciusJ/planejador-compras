using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Domain.Repositories.ShoppingList;

namespace PlanejadorCompras.Application.UseCases.ShoppingList;

public sealed class GetShoppingListByIdUseCase
{
    private readonly IShoppingListRepository _shoppingListRepository;

    public GetShoppingListByIdUseCase(IShoppingListRepository shoppingListRepository)
    {
        _shoppingListRepository = shoppingListRepository;
    }

    public async Task<ShoppingListResponseDto?> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(id, Guid.Empty);

        var shoppingList = await _shoppingListRepository.GetByIdAsync(id, cancellationToken);
        if (shoppingList is null)
        {
            return null;
        }

        return new ShoppingListResponseDto(
            shoppingList.Id,
            shoppingList.UserId,
            shoppingList.Name,
            shoppingList.Description,
            shoppingList.CreatedAt);
    }
}
