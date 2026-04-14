using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Domain.Repositories.ShoppingList;

namespace PlanejadorCompras.Application.UseCases.ShoppingList;

public sealed class GetShoppingListsByUserIdUseCase
{
    private readonly IShoppingListRepository _shoppingListRepository;

    public GetShoppingListsByUserIdUseCase(IShoppingListRepository shoppingListRepository)
    {
        _shoppingListRepository = shoppingListRepository;
    }

    public async Task<List<ShoppingListResponseDto>> ExecuteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(userId, Guid.Empty);

        var shoppingLists = await _shoppingListRepository.GetByUserIdAsync(userId, cancellationToken);
        return shoppingLists.Select(sl => new ShoppingListResponseDto(
            sl.Id,
            sl.UserId,
            sl.Name,
            sl.Description,
            sl.CreatedAt)).ToList();
    }
}
