using Moq;
using PlanejadorCompras.Application.Common.Dtos.Requests;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories;
using PlanejadorCompras.Domain.Repositories.ItemQuote;
using PlanejadorCompras.Domain.Repositories.ShoppingItem;
using ShoppingItemEntity = PlanejadorCompras.Domain.Entities.ShoppingItem;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ItemQuote.Update;

public sealed class UpdateItemQuoteTestHelper
{
    public UpdateItemQuoteTestHelper()
    {
        ItemQuoteRepositoryMock = new Mock<IItemQuoteRepository>();
        ShoppingItemRepositoryMock = new Mock<IShoppingItemRepository>();
        ShoppingListAccessServiceMock = new Mock<IShoppingListAccessService>();
        UnitOfWorkMock = new Mock<IUnitOfWork>();
        UnitOfWorkMock
            .Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    public static Guid DefaultShoppingItemId => Guid.Parse("66666666-6666-6666-6666-666666666666");

    public Mock<IItemQuoteRepository> ItemQuoteRepositoryMock { get; }

    public Mock<IShoppingItemRepository> ShoppingItemRepositoryMock { get; }

    public Mock<IShoppingListAccessService> ShoppingListAccessServiceMock { get; }

    public Mock<IUnitOfWork> UnitOfWorkMock { get; }

    public static ItemQuoteRequestDto CreateRequestDto(
        Guid? shoppingItemId = null,
        string supplierName = "Updated Supplier",
        decimal unitPrice = 175.50m)
    {
        return new ItemQuoteRequestDto(shoppingItemId ?? DefaultShoppingItemId, supplierName, unitPrice);
    }

    public static ShoppingItemEntity CreateShoppingItemEntity(
        Guid? shoppingListId = null,
        string name = "Monthly Tech Shopping Item",
        decimal quantity = 2,
        string unit = "pcs")
    {
        return ShoppingItemEntity.Create(shoppingListId ?? Guid.Parse("55555555-5555-5555-5555-555555555555"), name, quantity, unit);
    }
}
