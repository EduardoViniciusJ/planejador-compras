using Moq;
using PlanejadorCompras.Application.Common.Dtos.Requests;
using PlanejadorCompras.Domain.Repositories;
using PlanejadorCompras.Domain.Repositories.ShoppingItem;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ShoppingItem.Create;

public sealed class CreateShoppingItemTestHelper
{
    public CreateShoppingItemTestHelper()
    {
        ShoppingItemRepositoryMock = new Mock<IShoppingItemRepository>();
        UnitOfWorkMock = new Mock<IUnitOfWork>();
        UnitOfWorkMock
            .Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    public static Guid DefaultShoppingListId => Guid.Parse("55555555-5555-5555-5555-555555555555");

    public Mock<IShoppingItemRepository> ShoppingItemRepositoryMock { get; }

    public Mock<IUnitOfWork> UnitOfWorkMock { get; }

    public static ShoppingItemRequestDto CreateRequestDto(
        Guid? shoppingListId = null,
        string name = "Monthly Tech Shopping Item",
        decimal quantity = 2,
        string unit = "pcs")
    {
        return new ShoppingItemRequestDto(shoppingListId ?? DefaultShoppingListId, name, quantity, unit);
    }
}
