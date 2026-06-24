using PlanejadorCompras.Application.Common.Dtos.Requests;
using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories.ShoppingList;
using PlanejadorCompras.Domain.Repositories;
using ShoppingListEntity = PlanejadorCompras.Domain.Entities.ShoppingList;

namespace PlanejadorCompras.Application.UseCases.ShoppingList.Create;

public sealed class CreateShoppingListUseCase
{
    private readonly IShoppingListRepository _shoppingListRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public CreateShoppingListUseCase(
        IShoppingListRepository shoppingListRepository,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser)
    {
        _shoppingListRepository = shoppingListRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<ShoppingListResponseDto> ExecuteAsync(
        ShoppingListRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var shoppingList = ShoppingListEntity.Create(_currentUser.UserId, request.Name, request.Description);
        await _shoppingListRepository.AddAsync(shoppingList, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return new ShoppingListResponseDto(
            shoppingList.Id,
            shoppingList.Name,
            shoppingList.Description,
            shoppingList.CreatedAt);
    }
}
