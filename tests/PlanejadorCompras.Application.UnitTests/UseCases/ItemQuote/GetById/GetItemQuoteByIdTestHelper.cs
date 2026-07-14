using Moq;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories.ItemQuote;
using PlanejadorCompras.Domain.Repositories.ShoppingItem;
using ItemQuoteEntity = PlanejadorCompras.Domain.Entities.ItemQuote;
using ShoppingItemEntity = PlanejadorCompras.Domain.Entities.ShoppingItem;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ItemQuote.GetById;

public sealed class GetItemQuoteByIdTestHelper
{
    public GetItemQuoteByIdTestHelper()
    {
        ItemQuoteRepositoryMock = new Mock<IItemQuoteRepository>();
        ShoppingItemRepositoryMock = new Mock<IShoppingItemRepository>();
        ShoppingListAccessServiceMock = new Mock<IShoppingListAccessService>();
        SupplierAccessServiceMock = new Mock<ISupplierAccessService>();
        SupplierAccessServiceMock
            .Setup(x => x.GetForCurrentUserAsync(DefaultSupplier.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(DefaultSupplier);
    }

    public static Guid DefaultShoppingItemId => Guid.Parse("66666666-6666-6666-6666-666666666666");
    public static PlanejadorCompras.Domain.Entities.Supplier DefaultSupplier { get; } =
        PlanejadorCompras.Domain.Entities.Supplier.Create(Guid.NewGuid(), "Best Monitor Supplier");

    public Mock<IItemQuoteRepository> ItemQuoteRepositoryMock { get; }

    public Mock<IShoppingItemRepository> ShoppingItemRepositoryMock { get; }

    public Mock<IShoppingListAccessService> ShoppingListAccessServiceMock { get; }
    public Mock<ISupplierAccessService> SupplierAccessServiceMock { get; }

    public static ItemQuoteEntity CreateItemQuoteEntity(
        Guid? shoppingItemId = null,
        Guid? supplierId = null,
        decimal unitPrice = 199.90m)
    {
        return ItemQuoteEntity.Create(
            shoppingItemId ?? DefaultShoppingItemId,
            supplierId ?? DefaultSupplier.Id,
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
