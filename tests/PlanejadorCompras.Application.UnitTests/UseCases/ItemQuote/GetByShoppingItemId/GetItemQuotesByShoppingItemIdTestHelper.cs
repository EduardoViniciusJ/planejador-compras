using Moq;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories.ItemQuote;
using PlanejadorCompras.Domain.Repositories.ShoppingItem;
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
    }

    public static Guid DefaultShoppingItemId => Guid.Parse("66666666-6666-6666-6666-666666666666");

    public Mock<IItemQuoteRepository> ItemQuoteRepositoryMock { get; }

    public Mock<IShoppingItemRepository> ShoppingItemRepositoryMock { get; }

    public Mock<IShoppingListAccessService> ShoppingListAccessServiceMock { get; }

    public static ItemQuoteEntity CreateItemQuoteEntity(
        Guid? shoppingItemId = null,
        string supplierName = "Best Monitor Supplier",
        decimal unitPrice = 199.90m)
    {
        return ItemQuoteEntity.Create(shoppingItemId ?? DefaultShoppingItemId, supplierName, unitPrice);
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
