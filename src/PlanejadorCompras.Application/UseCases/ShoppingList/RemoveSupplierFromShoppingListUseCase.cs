using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories;
using PlanejadorCompras.Domain.Repositories.ShoppingListSupplier;

namespace PlanejadorCompras.Application.UseCases.ShoppingList;

public sealed class RemoveSupplierFromShoppingListUseCase(
    IShoppingListSupplierRepository shoppingListSupplierRepository,
    IShoppingListAccessService shoppingListAccessService,
    ISupplierAccessService supplierAccessService,
    IUnitOfWork unitOfWork)
{
    public async Task ExecuteAsync(
        Guid shoppingListId,
        Guid supplierId,
        CancellationToken cancellationToken = default)
    {
        await shoppingListAccessService.GetForCurrentUserAsync(shoppingListId, cancellationToken);
        await supplierAccessService.GetForCurrentUserAsync(supplierId, cancellationToken);

        if (!await shoppingListSupplierRepository.DeleteAsync(
                shoppingListId,
                supplierId,
                cancellationToken))
        {
            throw new NotFoundException(
                "Supplier is not assigned to this shopping list.",
                "shopping_list_supplier_not_found");
        }

        await unitOfWork.CommitAsync(cancellationToken);
    }
}
