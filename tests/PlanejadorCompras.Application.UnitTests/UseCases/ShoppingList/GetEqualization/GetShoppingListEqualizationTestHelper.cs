using Moq;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories.ItemQuote;
using PlanejadorCompras.Domain.Repositories.ShoppingItem;
using ShoppingListEntity = PlanejadorCompras.Domain.Entities.ShoppingList;
using ShoppingItemEntity = PlanejadorCompras.Domain.Entities.ShoppingItem;
using ItemQuoteEntity = PlanejadorCompras.Domain.Entities.ItemQuote;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ShoppingList.GetEqualization;

internal sealed class GetShoppingListEqualizationTestHelper
{
    public Mock<IShoppingListAccessService> ShoppingListAccessServiceMock { get; } = new();
    public Mock<IShoppingItemRepository> ShoppingItemRepositoryMock { get; } = new();
    public Mock<IItemQuoteRepository> ItemQuoteRepositoryMock { get; } = new();

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
        return ItemQuoteEntity.Create(itemId, supplier, price);
    }
}
