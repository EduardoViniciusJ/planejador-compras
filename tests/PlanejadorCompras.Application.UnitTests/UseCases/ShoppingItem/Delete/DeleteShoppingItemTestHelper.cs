using Moq;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories;
using PlanejadorCompras.Domain.Repositories.ShoppingItem;
using ShoppingListEntity = PlanejadorCompras.Domain.Entities.ShoppingList;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ShoppingItem.Delete;

public sealed class DeleteShoppingItemTestHelper
{
    public DeleteShoppingItemTestHelper()
    {
        ShoppingItemRepositoryMock = new Mock<IShoppingItemRepository>();
        ShoppingItemRepositoryMock
            .Setup(x => x.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        UnitOfWorkMock = new Mock<IUnitOfWork>();
        ShoppingListAccessServiceMock = new Mock<IShoppingListAccessService>();
        UnitOfWorkMock
            .Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    public static Guid DefaultShoppingItemId => Guid.Parse("33333333-3333-3333-3333-333333333333");

    public Mock<IShoppingItemRepository> ShoppingItemRepositoryMock { get; }

    public Mock<IUnitOfWork> UnitOfWorkMock { get; }

    public Mock<IShoppingListAccessService> ShoppingListAccessServiceMock { get; }

    public void SetupShoppingListAccess(Guid shoppingListId)
    {
        ShoppingListAccessServiceMock
            .Setup(x => x.GetForCurrentUserAsync(shoppingListId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ShoppingListEntity.Create(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Authorized List"));
    }
}
