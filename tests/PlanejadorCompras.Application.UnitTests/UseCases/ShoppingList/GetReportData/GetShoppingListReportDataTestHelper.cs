using Moq;
using PlanejadorCompras.Application.Services;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Application.UseCases.ShoppingList;
using PlanejadorCompras.Domain.Repositories.ItemQuote;
using PlanejadorCompras.Domain.Repositories.ShoppingItem;
using PlanejadorCompras.Domain.Repositories.ShoppingListSupplier;
using ItemQuoteEntity = PlanejadorCompras.Domain.Entities.ItemQuote;
using ShoppingItemEntity = PlanejadorCompras.Domain.Entities.ShoppingItem;
using ShoppingListEntity = PlanejadorCompras.Domain.Entities.ShoppingList;
using SupplierEntity = PlanejadorCompras.Domain.Entities.Supplier;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ShoppingList.GetReportData;

internal sealed class GetShoppingListReportDataTestHelper
{
    public static readonly SupplierEntity SupplierA =
        SupplierEntity.Create(Guid.NewGuid(), "Supplier A");
    public static readonly SupplierEntity SupplierB =
        SupplierEntity.Create(Guid.NewGuid(), "Supplier B");

    public GetShoppingListReportDataTestHelper(DateTimeOffset generatedAt)
    {
        TimeProvider = new FixedTimeProvider(generatedAt);
    }

    public Mock<IShoppingListAccessService> ShoppingListAccessServiceMock { get; } = new();
    public Mock<IShoppingItemRepository> ShoppingItemRepositoryMock { get; } = new();
    public Mock<IItemQuoteRepository> ItemQuoteRepositoryMock { get; } = new();
    public Mock<IShoppingListSupplierRepository> ShoppingListSupplierRepositoryMock { get; } = new();
    public TimeProvider TimeProvider { get; }

    public GetShoppingListReportDataUseCase CreateUseCase()
    {
        return new GetShoppingListReportDataUseCase(
            ShoppingListAccessServiceMock.Object,
            ShoppingItemRepositoryMock.Object,
            ItemQuoteRepositoryMock.Object,
            ShoppingListSupplierRepositoryMock.Object,
            new ShoppingListComparisonCalculator(),
            TimeProvider);
    }

    public void SetupData(
        Guid shoppingListId,
        IReadOnlyCollection<ShoppingItemEntity> items,
        IReadOnlyCollection<ItemQuoteEntity> quotes,
        IReadOnlyCollection<SupplierEntity>? suppliers = null)
    {
        ShoppingListAccessServiceMock
            .Setup(service => service.GetForCurrentUserAsync(
                shoppingListId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateShoppingList());
        ShoppingItemRepositoryMock
            .Setup(repository => repository.GetByShoppingListIdAsync(
                shoppingListId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(items.ToList());
        ItemQuoteRepositoryMock
            .Setup(repository => repository.GetByShoppingListIdAsync(
                shoppingListId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(quotes.ToList());
        ShoppingListSupplierRepositoryMock
            .Setup(repository => repository.GetSuppliersAsync(
                shoppingListId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((suppliers ?? GetQuotedSuppliers(quotes)).ToList());
    }

    public static ShoppingListEntity CreateShoppingList()
    {
        return ShoppingListEntity.Create(Guid.NewGuid(), "Office supplies", "Monthly purchase");
    }

    public static ShoppingItemEntity CreateItem(
        Guid shoppingListId,
        string name,
        decimal quantity)
    {
        return ShoppingItemEntity.Create(shoppingListId, name, quantity, "un");
    }

    public static ItemQuoteEntity CreateQuote(
        ShoppingItemEntity shoppingItem,
        SupplierEntity supplier,
        decimal unitPrice)
    {
        return ItemQuoteEntity.Create(shoppingItem.Id, supplier.Id, unitPrice);
    }

    private static IReadOnlyCollection<SupplierEntity> GetQuotedSuppliers(
        IEnumerable<ItemQuoteEntity> quotes)
    {
        var supplierIds = quotes
            .Select(quote => quote.SupplierId)
            .ToHashSet();

        return new[] { SupplierA, SupplierB }
            .Where(supplier => supplierIds.Contains(supplier.Id))
            .ToList();
    }

    private sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => utcNow;
    }
}
