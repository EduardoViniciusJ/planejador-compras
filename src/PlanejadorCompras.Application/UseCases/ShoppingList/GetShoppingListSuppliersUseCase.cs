using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories.ShoppingListSupplier;

namespace PlanejadorCompras.Application.UseCases.ShoppingList;

public sealed class GetShoppingListSuppliersUseCase(
    IShoppingListSupplierRepository shoppingListSupplierRepository,
    IShoppingListAccessService shoppingListAccessService)
{
    public async Task<IReadOnlyCollection<SupplierResponseDto>> ExecuteAsync(
        Guid shoppingListId,
        CancellationToken cancellationToken = default)
    {
        await shoppingListAccessService.GetForCurrentUserAsync(shoppingListId, cancellationToken);

        var suppliers = await shoppingListSupplierRepository.GetSuppliersAsync(
            shoppingListId,
            cancellationToken);

        return suppliers
            .Select(supplier => new SupplierResponseDto(supplier.Id, supplier.Name, supplier.CreatedAt))
            .ToList();
    }
}
