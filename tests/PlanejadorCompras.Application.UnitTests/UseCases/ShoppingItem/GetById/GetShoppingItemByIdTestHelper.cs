using Moq;
using PlanejadorCompras.Domain.Repositories.ShoppingItem;
using ShoppingItemEntity = PlanejadorCompras.Domain.Entities.ShoppingItem;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ShoppingItem.GetById;

public sealed class GetShoppingItemByIdTestHelper
{
    public GetShoppingItemByIdTestHelper()
    {
        ShoppingItemRepositoryMock = new Mock<IShoppingItemRepository>();
    }

    public static Guid DefaultShoppingListId => Guid.Parse("55555555-5555-5555-5555-555555555555");

    public Mock<IShoppingItemRepository> ShoppingItemRepositoryMock { get; }

    public static ShoppingItemEntity CreateShoppingItemEntity(
        Guid? shoppingListId = null,
        string name = "Monthly Tech Shopping Item",
        decimal quantity = 2,
        string unit = "pcs")
    {
        return ShoppingItemEntity.Create(shoppingListId ?? DefaultShoppingListId, name, quantity, unit);
    }
}
