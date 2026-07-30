using Moq;
using PlanejadorCompras.Application.Services;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories.ItemQuote;
using PlanejadorCompras.Domain.Repositories.ShoppingItem;
using PlanejadorCompras.Domain.Repositories.ShoppingListSupplier;
using ItemQuoteEntity = PlanejadorCompras.Domain.Entities.ItemQuote;
using ShoppingItemEntity = PlanejadorCompras.Domain.Entities.ShoppingItem;
using ShoppingListEntity = PlanejadorCompras.Domain.Entities.ShoppingList;
using SupplierEntity = PlanejadorCompras.Domain.Entities.Supplier;

namespace PlanejadorCompras.Application.UnitTests.UseCases.PurchaseOrder;

public sealed class PurchaseOrderDraftServiceTests
{
    [Fact]
    public async Task BuildAsync_ShouldUseOnlyLowestSupplierQuotesAndReportPartialCoverage()
    {
        var userId = Guid.NewGuid();
        var list = ShoppingListEntity.Create(userId, "Lista de teste");
        var supplier = SupplierEntity.Create(userId, "Fornecedor A");
        var otherSupplier = SupplierEntity.Create(userId, "Fornecedor B");
        var firstItem = ShoppingItemEntity.Create(list.Id, "Mouse", 2, "un");
        var secondItem = ShoppingItemEntity.Create(list.Id, "Teclado", 1, "un");

        var listAccess = new Mock<IShoppingListAccessService>();
        listAccess
            .Setup(service => service.GetForCurrentUserAsync(
                list.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);
        var supplierAccess = new Mock<ISupplierAccessService>();
        supplierAccess
            .Setup(service => service.GetForCurrentUserAsync(
                supplier.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(supplier);
        var links = new Mock<IShoppingListSupplierRepository>();
        links
            .Setup(repository => repository.ExistsAsync(
                list.Id,
                supplier.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var items = new Mock<IShoppingItemRepository>();
        items
            .Setup(repository => repository.GetByShoppingListIdAsync(
                list.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([firstItem, secondItem]);
        var quotes = new Mock<IItemQuoteRepository>();
        quotes
            .Setup(repository => repository.GetByShoppingListIdAsync(
                list.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                ItemQuoteEntity.Create(firstItem.Id, supplier.Id, 12m),
                ItemQuoteEntity.Create(firstItem.Id, supplier.Id, 10m),
                ItemQuoteEntity.Create(secondItem.Id, otherSupplier.Id, 5m)
            ]);
        var savedEqualizations = new Mock<ISavedEqualizationAccessService>();

        var service = new PurchaseOrderDraftService(
            listAccess.Object,
            supplierAccess.Object,
            links.Object,
            items.Object,
            quotes.Object,
            savedEqualizations.Object);

        var draft = await service.BuildAsync(list.Id, supplier.Id);

        Assert.False(draft.HasCompleteCoverage);
        Assert.Single(draft.Items);
        Assert.Equal(10m, draft.Items.Single().UnitPrice);
        Assert.Equal(20m, draft.TotalPrice);
    }
}
