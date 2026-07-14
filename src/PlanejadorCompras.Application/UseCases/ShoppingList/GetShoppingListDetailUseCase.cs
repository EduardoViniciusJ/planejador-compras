using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.Services.Interfaces;

namespace PlanejadorCompras.Application.UseCases.ShoppingList;

public sealed class GetShoppingListDetailUseCase
{
    private readonly ICurrentUser _currentUser;
    private readonly IShoppingListDetailQuery _shoppingListDetailQuery;

    public GetShoppingListDetailUseCase(
        ICurrentUser currentUser,
        IShoppingListDetailQuery shoppingListDetailQuery)
    {
        _currentUser = currentUser;
        _shoppingListDetailQuery = shoppingListDetailQuery;
    }

    public async Task<ShoppingListDetailResponseDto> ExecuteAsync(
        Guid shoppingListId,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(shoppingListId, Guid.Empty);

        var detail = await _shoppingListDetailQuery.GetByIdAsync(
            _currentUser.UserId,
            shoppingListId,
            cancellationToken);

        return detail ?? throw new NotFoundException(
            "Shopping list not found.",
            "shopping_list_not_found");
    }
}
