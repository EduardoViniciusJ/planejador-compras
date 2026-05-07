using Moq;
using PlanejadorCompras.Application.Common.Dtos.Requests;
using PlanejadorCompras.Domain.Repositories;
using PlanejadorCompras.Domain.Repositories.ShoppingList;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ShoppingList.Create;

public sealed class CreateShoppingListTestHelper
{
    public CreateShoppingListTestHelper()
    {
        ShoppingListRepositoryMock = new Mock<IShoppingListRepository>();
        UnitOfWorkMock = new Mock<IUnitOfWork>();
        UnitOfWorkMock
            .Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    public static Guid DefaultUserId => Guid.Parse("11111111-1111-1111-1111-111111111111");

    public Mock<IShoppingListRepository> ShoppingListRepositoryMock { get; }

    public Mock<IUnitOfWork> UnitOfWorkMock { get; }

    public static ShoppingListRequestDto CreateRequestDto(
        string name = "Monthly Tech Shopping",
        string? description = "Monitor, keyboard, and mouse")
    {
        return new ShoppingListRequestDto(name, description);
    }
}
