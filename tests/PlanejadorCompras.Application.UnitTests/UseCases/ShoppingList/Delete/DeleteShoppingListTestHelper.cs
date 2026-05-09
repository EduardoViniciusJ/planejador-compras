using Moq;
using PlanejadorCompras.Domain.Repositories;
using PlanejadorCompras.Domain.Repositories.ShoppingList;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ShoppingList.Delete;

public sealed class DeleteShoppingListTestHelper
{
    public DeleteShoppingListTestHelper()
    {
        ShoppingListRepositoryMock = new Mock<IShoppingListRepository>();
        UnitOfWorkMock = new Mock<IUnitOfWork>();
        UnitOfWorkMock
            .Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    public static Guid DefaultShoppingListId => Guid.Parse("44444444-4444-4444-4444-444444444444");

    public Mock<IShoppingListRepository> ShoppingListRepositoryMock { get; }

    public Mock<IUnitOfWork> UnitOfWorkMock { get; }
}
