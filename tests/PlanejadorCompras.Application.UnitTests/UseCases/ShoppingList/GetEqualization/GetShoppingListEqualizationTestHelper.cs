using Moq;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories.ItemQuote;
using PlanejadorCompras.Domain.Repositories.ShoppingItem;
using PlanejadorCompras.Domain.Repositories.Supplier;
using ShoppingListEntity = PlanejadorCompras.Domain.Entities.ShoppingList;
using ShoppingItemEntity = PlanejadorCompras.Domain.Entities.ShoppingItem;
using ItemQuoteEntity = PlanejadorCompras.Domain.Entities.ItemQuote;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ShoppingList.GetEqualization;

internal sealed class GetShoppingListEqualizationTestHelper
{
    private static readonly PlanejadorCompras.Domain.Entities.Supplier SupplierA =
        PlanejadorCompras.Domain.Entities.Supplier.Create(Guid.NewGuid(), "Supplier A");
    private static readonly PlanejadorCompras.Domain.Entities.Supplier SupplierB =
        PlanejadorCompras.Domain.Entities.Supplier.Create(Guid.NewGuid(), "Supplier B");

    public GetShoppingListEqualizationTestHelper()
    {
        SupplierRepositoryMock
            .Setup(repository => repository.GetByIdsAsync(
                It.IsAny<IEnumerable<Guid>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((IEnumerable<Guid> ids, CancellationToken _) =>
                new[] { SupplierA, SupplierB }.Where(supplier => ids.Contains(supplier.Id)).ToList());
    }

    public Mock<IShoppingListAccessService> ShoppingListAccessServiceMock { get; } = new();
    public Mock<IShoppingItemRepository> ShoppingItemRepositoryMock { get; } = new();
    public Mock<IItemQuoteRepository> ItemQuoteRepositoryMock { get; } = new();
    public Mock<ISupplierRepository> SupplierRepositoryMock { get; } = new();

    public static ShoppingListEntity CreateShoppingList()
    {
        return ShoppingListEntity.Create(Guid.NewGuid(), "Test List", "Desc");
    }

    public static ShoppingItemEntity CreateShoppingItem(Guid listId, string name, decimal quantity)
    {
        return ShoppingItemEntity.Create(listId, name, quantity, "Un");
    }

    public static ItemQuoteEntity CreateQuote(Guid itemId, string supplier, decimal price)
    {
        var supplierEntity = supplier == SupplierA.Name ? SupplierA : SupplierB;
        return ItemQuoteEntity.Create(itemId, supplierEntity.Id, price);
    }
}
