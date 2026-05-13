using Moq;
using PlanejadorCompras.Application.Common.Dtos.Requests;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories;
using PlanejadorCompras.Domain.Repositories.ShoppingList;
using ShoppingListEntity = PlanejadorCompras.Domain.Entities.ShoppingList;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ShoppingList.Update;

public sealed class UpdateShoppingListTestHelper
{
    public UpdateShoppingListTestHelper()
    {
        ShoppingListRepositoryMock = new Mock<IShoppingListRepository>();
        CurrentUserMock = new Mock<ICurrentUser>();
        CurrentUserMock.Setup(x => x.UserId).Returns(DefaultUserId);
        UnitOfWorkMock = new Mock<IUnitOfWork>();
        UnitOfWorkMock
            .Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    public static Guid DefaultUserId => Guid.Parse("11111111-1111-1111-1111-111111111111");

    public Mock<IShoppingListRepository> ShoppingListRepositoryMock { get; }
    public Mock<ICurrentUser> CurrentUserMock { get; }

    public Mock<IUnitOfWork> UnitOfWorkMock { get; }

    public static ShoppingListRequestDto CreateRequestDto(
        string name = "Updated Tech Shopping",
        string? description = "Monitor arm and USB hub")
    {
        return new ShoppingListRequestDto(name, description);
    }

    public static ShoppingListEntity CreateShoppingListEntity(
        Guid? userId = null,
        string name = "Monthly Tech Shopping",
        string? description = "Monitor, keyboard, and mouse")
    {
        return ShoppingListEntity.Create(userId ?? DefaultUserId, name, description);
    }
}
