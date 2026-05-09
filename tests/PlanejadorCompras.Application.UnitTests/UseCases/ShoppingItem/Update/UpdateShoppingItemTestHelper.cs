using Moq;
using PlanejadorCompras.Application.Common.Dtos.Requests;
using PlanejadorCompras.Domain.Repositories;
using PlanejadorCompras.Domain.Repositories.ShoppingItem;
using ShoppingItemEntity = PlanejadorCompras.Domain.Entities.ShoppingItem;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ShoppingItem.Update;

public sealed class UpdateShoppingItemTestHelper
{
    public UpdateShoppingItemTestHelper()
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
        string name = "Updated Tech Shopping Item",
        decimal quantity = 3,
        string unit = "pcs")
    {
        return new ShoppingItemRequestDto(shoppingListId ?? DefaultShoppingListId, name, quantity, unit);
    }

    public static ShoppingItemEntity CreateShoppingItemEntity(
        Guid? shoppingListId = null,
        string name = "Monthly Tech Shopping Item",
        decimal quantity = 2,
        string unit = "pcs")
    {
        return ShoppingItemEntity.Create(shoppingListId ?? DefaultShoppingListId, name, quantity, unit);
    }
}
