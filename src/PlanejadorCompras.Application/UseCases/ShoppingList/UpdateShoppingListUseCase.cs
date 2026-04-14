using PlanejadorCompras.Application.Common.Dtos.Requests;
using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Domain.Repositories.ShoppingList;
using PlanejadorCompras.Domain.Repositories;

namespace PlanejadorCompras.Application.UseCases.ShoppingList;

public sealed class UpdateShoppingListUseCase
{
    private readonly IShoppingListRepository _shoppingListRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateShoppingListUseCase(
        IShoppingListRepository shoppingListRepository,
        IUnitOfWork unitOfWork)
    {
        _shoppingListRepository = shoppingListRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ShoppingListResponseDto?> ExecuteAsync(
        Guid id,
        ShoppingListRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentOutOfRangeException.ThrowIfEqual(id, Guid.Empty);

        var shoppingList = await _shoppingListRepository.GetByIdAsync(id, cancellationToken);
        if (shoppingList is null)
        {
            return null;
        }

        await _shoppingListRepository.UpdateAsync(shoppingList, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return new ShoppingListResponseDto(
            shoppingList.Id,
            shoppingList.UserId,
            shoppingList.Name,
            shoppingList.Description,
            shoppingList.CreatedAt);
    }
}
