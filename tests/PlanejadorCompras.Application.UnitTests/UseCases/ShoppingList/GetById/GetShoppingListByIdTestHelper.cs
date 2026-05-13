using Moq;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories.ShoppingList;
using ShoppingListEntity = PlanejadorCompras.Domain.Entities.ShoppingList;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ShoppingList.GetById;

public sealed class GetShoppingListByIdTestHelper
{
    public GetShoppingListByIdTestHelper()
    {
        ShoppingListRepositoryMock = new Mock<IShoppingListRepository>();
        CurrentUserMock = new Mock<ICurrentUser>();
        CurrentUserMock.Setup(x => x.UserId).Returns(DefaultUserId);
    }

    public static Guid DefaultUserId => Guid.Parse("11111111-1111-1111-1111-111111111111");

    public Mock<IShoppingListRepository> ShoppingListRepositoryMock { get; }
    public Mock<ICurrentUser> CurrentUserMock { get; }

    public static ShoppingListEntity CreateShoppingListEntity(
        Guid? userId = null,
        string name = "Monthly Tech Shopping",
        string? description = "Monitor, keyboard, and mouse")
    {
        return ShoppingListEntity.Create(userId ?? DefaultUserId, name, description);
    }
}
