using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories;
using PlanejadorCompras.Domain.Repositories.ShoppingListSupplier;
using ShoppingListSupplierEntity = PlanejadorCompras.Domain.Entities.ShoppingListSupplier;

namespace PlanejadorCompras.Application.UseCases.ShoppingList;

public sealed class AddSupplierToShoppingListUseCase(
    IShoppingListSupplierRepository shoppingListSupplierRepository,
    IShoppingListAccessService shoppingListAccessService,
    ISupplierAccessService supplierAccessService,
    IUnitOfWork unitOfWork)
{
    public async Task<SupplierResponseDto> ExecuteAsync(
        Guid shoppingListId,
        Guid supplierId,
        CancellationToken cancellationToken = default)
    {
        await shoppingListAccessService.GetForCurrentUserAsync(shoppingListId, cancellationToken);
        var supplier = await supplierAccessService.GetForCurrentUserAsync(supplierId, cancellationToken);

        if (!await shoppingListSupplierRepository.ExistsAsync(
                shoppingListId,
                supplierId,
                cancellationToken))
        {
            await shoppingListSupplierRepository.AddAsync(
                ShoppingListSupplierEntity.Create(shoppingListId, supplierId),
                cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);
        }

        return new SupplierResponseDto(supplier.Id, supplier.Name, supplier.CreatedAt);
    }
}
