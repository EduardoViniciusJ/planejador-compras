using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories.ShoppingList;

namespace PlanejadorCompras.Application.UseCases.ShoppingList;

public sealed class GetShoppingListsByUserIdUseCase
{
    private readonly IShoppingListRepository _shoppingListRepository;
    private readonly ICurrentUser _currentUser;

    public GetShoppingListsByUserIdUseCase(
        IShoppingListRepository shoppingListRepository,
        ICurrentUser currentUser)
    {
        _shoppingListRepository = shoppingListRepository;
        _currentUser = currentUser;
    }

    public async Task<List<ShoppingListResponseDto>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var shoppingLists = await _shoppingListRepository.GetByUserIdAsync(_currentUser.UserId, cancellationToken);
        return shoppingLists.Select(sl => new ShoppingListResponseDto(
            sl.Id,
            sl.Name,
            sl.Description,
            sl.CreatedAt)).ToList();
    }
}
