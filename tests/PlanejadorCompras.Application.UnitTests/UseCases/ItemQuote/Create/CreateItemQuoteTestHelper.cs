using Moq;
using PlanejadorCompras.Application.Common.Dtos.Requests;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories;
using PlanejadorCompras.Domain.Repositories.ItemQuote;
using PlanejadorCompras.Domain.Repositories.ShoppingItem;
using ShoppingItemEntity = PlanejadorCompras.Domain.Entities.ShoppingItem;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ItemQuote.Create;

public sealed class CreateItemQuoteTestHelper
{
    public CreateItemQuoteTestHelper()
    {
        ItemQuoteRepositoryMock = new Mock<IItemQuoteRepository>();
        ShoppingItemRepositoryMock = new Mock<IShoppingItemRepository>();
        ShoppingListAccessServiceMock = new Mock<IShoppingListAccessService>();
        SupplierAccessServiceMock = new Mock<ISupplierAccessService>();
        SetupSupplierAccess();
        UnitOfWorkMock = new Mock<IUnitOfWork>();
        UnitOfWorkMock
            .Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    public static Guid DefaultShoppingItemId => Guid.Parse("66666666-6666-6666-6666-666666666666");
    public static PlanejadorCompras.Domain.Entities.Supplier DefaultSupplier { get; } =
        PlanejadorCompras.Domain.Entities.Supplier.Create(
            Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            "Best Monitor Supplier");

    public Mock<IItemQuoteRepository> ItemQuoteRepositoryMock { get; }

    public Mock<IShoppingItemRepository> ShoppingItemRepositoryMock { get; }

    public Mock<IShoppingListAccessService> ShoppingListAccessServiceMock { get; }
    public Mock<ISupplierAccessService> SupplierAccessServiceMock { get; }

    public Mock<IUnitOfWork> UnitOfWorkMock { get; }

    public static ItemQuoteRequestDto CreateRequestDto(
        Guid? shoppingItemId = null,
        Guid? supplierId = null,
        decimal unitPrice = 199.90m)
    {
        return new ItemQuoteRequestDto(
            shoppingItemId ?? DefaultShoppingItemId,
            supplierId ?? DefaultSupplier.Id,
            unitPrice);
    }

    public static ShoppingItemEntity CreateShoppingItemEntity(
        Guid? shoppingListId = null,
        string name = "Monthly Tech Shopping Item",
        decimal quantity = 2,
        string unit = "pcs")
    {
        return ShoppingItemEntity.Create(shoppingListId ?? Guid.Parse("55555555-5555-5555-5555-555555555555"), name, quantity, unit);
    }

    public void SetupShoppingListAccess(Guid shoppingListId)
    {
        ShoppingListAccessServiceMock
            .Setup(x => x.GetForCurrentUserAsync(shoppingListId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(PlanejadorCompras.Domain.Entities.ShoppingList.Create(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Authorized List"));
    }

    private void SetupSupplierAccess()
    {
        SupplierAccessServiceMock
            .Setup(x => x.GetForCurrentUserAsync(DefaultSupplier.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(DefaultSupplier);
    }
}
