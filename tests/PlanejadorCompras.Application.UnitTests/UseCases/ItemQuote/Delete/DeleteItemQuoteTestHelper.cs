using Moq;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories;
using PlanejadorCompras.Domain.Repositories.ItemQuote;
using PlanejadorCompras.Domain.Repositories.ShoppingItem;
using ShoppingItemEntity = PlanejadorCompras.Domain.Entities.ShoppingItem;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ItemQuote.Delete;

public sealed class DeleteItemQuoteTestHelper
{
    public DeleteItemQuoteTestHelper()
    {
        ItemQuoteRepositoryMock = new Mock<IItemQuoteRepository>();
        ItemQuoteRepositoryMock
            .Setup(x => x.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        ShoppingItemRepositoryMock = new Mock<IShoppingItemRepository>();
        ShoppingListAccessServiceMock = new Mock<IShoppingListAccessService>();
        UnitOfWorkMock = new Mock<IUnitOfWork>();
        UnitOfWorkMock
            .Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    public static Guid DefaultItemQuoteId => Guid.Parse("77777777-7777-7777-7777-777777777777");

    public Mock<IItemQuoteRepository> ItemQuoteRepositoryMock { get; }

    public Mock<IShoppingItemRepository> ShoppingItemRepositoryMock { get; }

    public Mock<IShoppingListAccessService> ShoppingListAccessServiceMock { get; }

    public Mock<IUnitOfWork> UnitOfWorkMock { get; }

    public static ShoppingItemEntity CreateShoppingItemEntity(
        Guid? shoppingListId = null,
        string name = "Monthly Tech Shopping Item",
        decimal quantity = 2,
        string unit = "pcs")
    {
        return ShoppingItemEntity.Create(shoppingListId ?? Guid.Parse("55555555-5555-5555-5555-555555555555"), name, quantity, unit);
    }
}
