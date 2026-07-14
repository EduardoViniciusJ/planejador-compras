using Moq;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.UseCases.ShoppingList;
using Xunit;
using ShoppingItemEntity = PlanejadorCompras.Domain.Entities.ShoppingItem;
using ItemQuoteEntity = PlanejadorCompras.Domain.Entities.ItemQuote;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ShoppingList.BestSupplierBudget;

public sealed class CalculateBestSupplierBudgetUseCaseTests
{
    private readonly CalculateBestSupplierBudgetTestHelper _helper;
    private readonly CalculateBestSupplierBudgetUseCase _handler;

    public CalculateBestSupplierBudgetUseCaseTests()
    {
        _helper = new CalculateBestSupplierBudgetTestHelper();
        _handler = new CalculateBestSupplierBudgetUseCase(
            _helper.ShoppingListAccessServiceMock.Object,
            _helper.ShoppingItemRepositoryMock.Object,
            _helper.ItemQuoteRepositoryMock.Object,
            _helper.SupplierRepositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnBestSupplier_WhenQuotesExist()
    {
        var listId = Guid.NewGuid();
        var item1 = CalculateBestSupplierBudgetTestHelper.CreateShoppingItem(listId, "Item 1", 2m);
        var item2 = CalculateBestSupplierBudgetTestHelper.CreateShoppingItem(listId, "Item 2", 1m);
        
        var items = new List<ShoppingItemEntity> { item1, item2 };

        var quotes = new List<ItemQuoteEntity>
        {
            // Supplier A: (2 * 10) + (1 * 5) = 25
            CalculateBestSupplierBudgetTestHelper.CreateQuote(item1.Id, "Supplier A", 10m),
            CalculateBestSupplierBudgetTestHelper.CreateQuote(item2.Id, "Supplier A", 5m),

            // Supplier B: (2 * 8) + (1 * 8) = 24 -> Winner!
            CalculateBestSupplierBudgetTestHelper.CreateQuote(item1.Id, "Supplier B", 8m),
            CalculateBestSupplierBudgetTestHelper.CreateQuote(item2.Id, "Supplier B", 8m)
        };

        _helper.ShoppingListAccessServiceMock
            .Setup(x => x.GetForCurrentUserAsync(listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CalculateBestSupplierBudgetTestHelper.CreateShoppingList());

        _helper.ShoppingItemRepositoryMock
            .Setup(x => x.GetByShoppingListIdAsync(listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(items);

        _helper.ItemQuoteRepositoryMock
            .Setup(x => x.GetByShoppingListIdAsync(listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(quotes);

        var result = await _handler.ExecuteAsync(listId);

        Assert.NotNull(result);
        Assert.Equal(listId, result.ShoppingListId);
        Assert.Equal("Supplier B", result.BestSupplierName);
        Assert.Equal(24m, result.TotalPrice);
        Assert.Equal(2, result.Items.Count());
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnZeroAndNull_WhenNoQuotesExist()
    {
        var listId = Guid.NewGuid();

        _helper.ShoppingListAccessServiceMock
            .Setup(x => x.GetForCurrentUserAsync(listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CalculateBestSupplierBudgetTestHelper.CreateShoppingList());

        _helper.ShoppingItemRepositoryMock
            .Setup(x => x.GetByShoppingListIdAsync(listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ShoppingItemEntity>());

        _helper.ItemQuoteRepositoryMock
            .Setup(x => x.GetByShoppingListIdAsync(listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ItemQuoteEntity>());

        var result = await _handler.ExecuteAsync(listId);

        Assert.NotNull(result);
        Assert.Null(result.BestSupplierName);
        Assert.Equal(0m, result.TotalPrice);
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnNoSupplier_WhenNoSupplierCoversEveryItem()
    {
        var listId = Guid.NewGuid();
        var item1 = CalculateBestSupplierBudgetTestHelper.CreateShoppingItem(listId, "Item 1", 2m);
        var item2 = CalculateBestSupplierBudgetTestHelper.CreateShoppingItem(listId, "Item 2", 1m);

        _helper.ShoppingListAccessServiceMock
            .Setup(x => x.GetForCurrentUserAsync(listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CalculateBestSupplierBudgetTestHelper.CreateShoppingList());
        _helper.ShoppingItemRepositoryMock
            .Setup(x => x.GetByShoppingListIdAsync(listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ShoppingItemEntity> { item1, item2 });
        _helper.ItemQuoteRepositoryMock
            .Setup(x => x.GetByShoppingListIdAsync(listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ItemQuoteEntity>
            {
                CalculateBestSupplierBudgetTestHelper.CreateQuote(item1.Id, "Supplier A", 10m),
                CalculateBestSupplierBudgetTestHelper.CreateQuote(item2.Id, "Supplier B", 5m)
            });

        var result = await _handler.ExecuteAsync(listId);

        Assert.Null(result.BestSupplierName);
        Assert.Equal(0m, result.TotalPrice);
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldUseLowestQuote_WhenSupplierQuotedItemMoreThanOnce()
    {
        var listId = Guid.NewGuid();
        var item = CalculateBestSupplierBudgetTestHelper.CreateShoppingItem(listId, "Item", 2m);

        _helper.ShoppingListAccessServiceMock
            .Setup(x => x.GetForCurrentUserAsync(listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CalculateBestSupplierBudgetTestHelper.CreateShoppingList());
        _helper.ShoppingItemRepositoryMock
            .Setup(x => x.GetByShoppingListIdAsync(listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ShoppingItemEntity> { item });
        _helper.ItemQuoteRepositoryMock
            .Setup(x => x.GetByShoppingListIdAsync(listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ItemQuoteEntity>
            {
                CalculateBestSupplierBudgetTestHelper.CreateQuote(item.Id, "Supplier A", 10m),
                CalculateBestSupplierBudgetTestHelper.CreateQuote(item.Id, "Supplier A", 8m)
            });

        var result = await _handler.ExecuteAsync(listId);

        Assert.Equal("Supplier A", result.BestSupplierName);
        Assert.Equal(16m, result.TotalPrice);
        Assert.Equal(8m, Assert.Single(result.Items).UnitPrice);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowNotFoundException_WhenAccessServiceThrows()
    {
        var listId = Guid.NewGuid();

        _helper.ShoppingListAccessServiceMock
            .Setup(x => x.GetForCurrentUserAsync(listId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("List not found"));

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.ExecuteAsync(listId));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowArgumentOutOfRangeException_WhenIdIsEmpty()
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _handler.ExecuteAsync(Guid.Empty));
    }
}
