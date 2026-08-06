using PlanejadorCompras.Application.Features.ShoppingLists.Contracts;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories.ShoppingList;
using PlanejadorCompras.Domain.Repositories;

namespace PlanejadorCompras.Application.UseCases.ShoppingList;

public sealed class UpdateShoppingListUseCase
{
    private readonly IShoppingListRepository _shoppingListRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IShoppingListAccessService _shoppingListAccessService;

    public UpdateShoppingListUseCase(
        IShoppingListRepository shoppingListRepository,
        IUnitOfWork unitOfWork,
        IShoppingListAccessService shoppingListAccessService)
    {
        _shoppingListRepository = shoppingListRepository;
        _unitOfWork = unitOfWork;
        _shoppingListAccessService = shoppingListAccessService;
    }

    public async Task<ShoppingListResponseDto> ExecuteAsync(
        Guid id,
        ShoppingListRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var shoppingList = await _shoppingListAccessService.GetForCurrentUserAsync(id, cancellationToken);

        shoppingList.Update(request.Name, request.Description);
        await _shoppingListRepository.UpdateAsync(shoppingList, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return new ShoppingListResponseDto(
            shoppingList.Id,
            shoppingList.Name,
            shoppingList.Description,
            shoppingList.CreatedAt);
    }
}
