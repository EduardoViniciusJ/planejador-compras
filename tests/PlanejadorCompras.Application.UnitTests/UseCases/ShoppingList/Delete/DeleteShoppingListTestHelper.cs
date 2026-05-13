using Moq;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories;
using PlanejadorCompras.Domain.Repositories.ShoppingList;
using ShoppingListEntity = PlanejadorCompras.Domain.Entities.ShoppingList;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ShoppingList.Delete;

public sealed class DeleteShoppingListTestHelper
{
    public DeleteShoppingListTestHelper()
    {
        ShoppingListRepositoryMock = new Mock<IShoppingListRepository>();
        CurrentUserMock = new Mock<ICurrentUser>();
        CurrentUserMock.Setup(x => x.UserId).Returns(DefaultUserId);
        ShoppingListRepositoryMock
            .Setup(x => x.GetByIdAsync(DefaultShoppingListId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateShoppingListEntity());
        ShoppingListRepositoryMock
            .Setup(x => x.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        UnitOfWorkMock = new Mock<IUnitOfWork>();
        UnitOfWorkMock
            .Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    public static Guid DefaultShoppingListId => Guid.Parse("44444444-4444-4444-4444-444444444444");
    public static Guid DefaultUserId => Guid.Parse("11111111-1111-1111-1111-111111111111");

    public Mock<IShoppingListRepository> ShoppingListRepositoryMock { get; }
    public Mock<ICurrentUser> CurrentUserMock { get; }

    public Mock<IUnitOfWork> UnitOfWorkMock { get; }

    public static ShoppingListEntity CreateShoppingListEntity(Guid? userId = null)
    {
        return ShoppingListEntity.Create(userId ?? DefaultUserId, "Monthly Tech Shopping", "Monitor and keyboard");
    }
}
