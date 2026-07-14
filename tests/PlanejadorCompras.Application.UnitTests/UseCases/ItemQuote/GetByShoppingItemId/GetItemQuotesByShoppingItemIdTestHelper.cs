using Moq;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories.ItemQuote;
using PlanejadorCompras.Domain.Repositories.ShoppingItem;
using PlanejadorCompras.Domain.Repositories.Supplier;
using ItemQuoteEntity = PlanejadorCompras.Domain.Entities.ItemQuote;
using ShoppingItemEntity = PlanejadorCompras.Domain.Entities.ShoppingItem;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ItemQuote.GetByShoppingItemId;

public sealed class GetItemQuotesByShoppingItemIdTestHelper
{
    public GetItemQuotesByShoppingItemIdTestHelper()
    {
        ItemQuoteRepositoryMock = new Mock<IItemQuoteRepository>();
        ShoppingItemRepositoryMock = new Mock<IShoppingItemRepository>();
        ShoppingListAccessServiceMock = new Mock<IShoppingListAccessService>();
        SupplierRepositoryMock = new Mock<ISupplierRepository>();
        SupplierRepositoryMock
            .Setup(repository => repository.GetByIdsAsync(
                It.IsAny<IEnumerable<Guid>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((IEnumerable<Guid> ids, CancellationToken _) =>
                new[] { SupplierA, SupplierB }.Where(supplier => ids.Contains(supplier.Id)).ToList());
    }

    public static Guid DefaultShoppingItemId => Guid.Parse("66666666-6666-6666-6666-666666666666");
    public static PlanejadorCompras.Domain.Entities.Supplier SupplierA { get; } =
        PlanejadorCompras.Domain.Entities.Supplier.Create(Guid.NewGuid(), "Supplier A");
    public static PlanejadorCompras.Domain.Entities.Supplier SupplierB { get; } =
        PlanejadorCompras.Domain.Entities.Supplier.Create(Guid.NewGuid(), "Supplier B");

    public Mock<IItemQuoteRepository> ItemQuoteRepositoryMock { get; }

    public Mock<IShoppingItemRepository> ShoppingItemRepositoryMock { get; }

    public Mock<IShoppingListAccessService> ShoppingListAccessServiceMock { get; }
    public Mock<ISupplierRepository> SupplierRepositoryMock { get; }

    public static ItemQuoteEntity CreateItemQuoteEntity(
        Guid? shoppingItemId = null,
        Guid? supplierId = null,
        decimal unitPrice = 199.90m)
    {
        return ItemQuoteEntity.Create(
            shoppingItemId ?? DefaultShoppingItemId,
            supplierId ?? SupplierA.Id,
            unitPrice);
    }

    public static ShoppingItemEntity CreateShoppingItemEntity(
        Guid? shoppingListId = null,
        string name = "Monthly Tech Shopping Item",
        decimal quantity = 2,
        string unit = "pcs")
    {
        return ShoppingItemEntity.Create(shoppingListId ?? Guid.Parse("55555555-5555-5555-5555-555555555555"), name, quantity, unit);
    }
}
