using PlanejadorCompras.Application.Common.Dtos.Requests;
using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Entities;
using PlanejadorCompras.Domain.Repositories.ShoppingList;
using PlanejadorCompras.Domain.Repositories;

namespace PlanejadorCompras.Application.UseCases.ShoppingList;

public sealed class CreateShoppingListUseCase
{
    private readonly IShoppingListRepository _shoppingListRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateShoppingListUseCase(
        IShoppingListRepository shoppingListRepository,
        IUnitOfWork unitOfWork)
    {
        _shoppingListRepository = shoppingListRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ShoppingListResponseDto> ExecuteAsync(
        ShoppingListRequestDto request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentOutOfRangeException.ThrowIfEqual(userId, Guid.Empty);

        var shoppingList = ShoppingList.Create(userId, request.Name, request.Description);
        await _shoppingListRepository.AddAsync(shoppingList, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return new ShoppingListResponseDto(
            shoppingList.Id,
            shoppingList.UserId,
            shoppingList.Name,
            shoppingList.Description,
            shoppingList.CreatedAt);
    }
}
