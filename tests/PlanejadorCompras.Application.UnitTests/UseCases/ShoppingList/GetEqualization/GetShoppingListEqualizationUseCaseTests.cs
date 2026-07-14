using Moq;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.UseCases.ShoppingList;
using Xunit;
using ShoppingItemEntity = PlanejadorCompras.Domain.Entities.ShoppingItem;
using ItemQuoteEntity = PlanejadorCompras.Domain.Entities.ItemQuote;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ShoppingList.GetEqualization;

public sealed class GetShoppingListEqualizationUseCaseTests
{
    private readonly GetShoppingListEqualizationTestHelper _helper;
    private readonly GetShoppingListEqualizationUseCase _handler;

    public GetShoppingListEqualizationUseCaseTests()
    {
        _helper = new GetShoppingListEqualizationTestHelper();
        _handler = new GetShoppingListEqualizationUseCase(
            _helper.ShoppingListAccessServiceMock.Object,
            _helper.ShoppingItemRepositoryMock.Object,
            _helper.ItemQuoteRepositoryMock.Object,
            _helper.SupplierRepositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnEqualizationMatrix_WhenQuotesExist()
    {
        var listId = Guid.NewGuid();
        var item1 = GetShoppingListEqualizationTestHelper.CreateShoppingItem(listId, "Item 1", 10m);
        var item2 = GetShoppingListEqualizationTestHelper.CreateShoppingItem(listId, "Item 2", 5m);
        
        var items = new List<ShoppingItemEntity> { item1, item2 };

        var quotes = new List<ItemQuoteEntity>
        {
            GetShoppingListEqualizationTestHelper.CreateQuote(item1.Id, "Supplier B", 30m),
            GetShoppingListEqualizationTestHelper.CreateQuote(item1.Id, "Supplier A", 32.5m),
            GetShoppingListEqualizationTestHelper.CreateQuote(item2.Id, "Supplier A", 120m)
        };

        _helper.ShoppingListAccessServiceMock
            .Setup(x => x.GetForCurrentUserAsync(listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(GetShoppingListEqualizationTestHelper.CreateShoppingList());

        _helper.ShoppingItemRepositoryMock
            .Setup(x => x.GetByShoppingListIdAsync(listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(items);

        _helper.ItemQuoteRepositoryMock
            .Setup(x => x.GetByShoppingListIdAsync(listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(quotes);

        var result = await _handler.ExecuteAsync(listId);

        Assert.NotNull(result);
        Assert.Equal(listId, result.ShoppingListId);
        
        Assert.Equal(2, result.Suppliers.Count());
        Assert.Equal("Supplier A", result.Suppliers.ElementAt(0));
        Assert.Equal("Supplier B", result.Suppliers.ElementAt(1));

        Assert.Equal(2, result.Items.Count());
        
        var item1Row = result.Items.First(i => i.ShoppingItemId == item1.Id);
        Assert.Equal(2, item1Row.Quotes.Count());
        Assert.Equal(325m, item1Row.Quotes.First(q => q.SupplierName == "Supplier A").TotalPrice);
        
        var item2Row = result.Items.First(i => i.ShoppingItemId == item2.Id);
        Assert.Single(item2Row.Quotes);
        Assert.Equal(600m, item2Row.Quotes.First(q => q.SupplierName == "Supplier A").TotalPrice);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnEmptyMatrix_WhenNoQuotesExist()
    {
        var listId = Guid.NewGuid();
        var item1 = GetShoppingListEqualizationTestHelper.CreateShoppingItem(listId, "Item 1", 10m);
        var items = new List<ShoppingItemEntity> { item1 };

        _helper.ShoppingListAccessServiceMock
            .Setup(x => x.GetForCurrentUserAsync(listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(GetShoppingListEqualizationTestHelper.CreateShoppingList());

        _helper.ShoppingItemRepositoryMock
            .Setup(x => x.GetByShoppingListIdAsync(listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(items);

        _helper.ItemQuoteRepositoryMock
            .Setup(x => x.GetByShoppingListIdAsync(listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ItemQuoteEntity>());

        var result = await _handler.ExecuteAsync(listId);

        Assert.NotNull(result);
        Assert.Equal(listId, result.ShoppingListId);
        Assert.Empty(result.Suppliers);
        Assert.Single(result.Items);
        Assert.Empty(result.Items.First().Quotes);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldUseLowestQuotePerSupplierAndItem()
    {
        var listId = Guid.NewGuid();
        var item = GetShoppingListEqualizationTestHelper.CreateShoppingItem(listId, "Item", 3m);

        _helper.ShoppingListAccessServiceMock
            .Setup(x => x.GetForCurrentUserAsync(listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(GetShoppingListEqualizationTestHelper.CreateShoppingList());
        _helper.ShoppingItemRepositoryMock
            .Setup(x => x.GetByShoppingListIdAsync(listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ShoppingItemEntity> { item });
        _helper.ItemQuoteRepositoryMock
            .Setup(x => x.GetByShoppingListIdAsync(listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ItemQuoteEntity>
            {
                GetShoppingListEqualizationTestHelper.CreateQuote(item.Id, "Supplier A", 12m),
                GetShoppingListEqualizationTestHelper.CreateQuote(item.Id, "Supplier A", 10m)
            });

        var result = await _handler.ExecuteAsync(listId);
        var quote = Assert.Single(Assert.Single(result.Items).Quotes);

        Assert.Equal(10m, quote.UnitPrice);
        Assert.Equal(30m, quote.TotalPrice);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnEmpty_WhenNoItemsExist()
    {
        var listId = Guid.NewGuid();

        _helper.ShoppingListAccessServiceMock
            .Setup(x => x.GetForCurrentUserAsync(listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(GetShoppingListEqualizationTestHelper.CreateShoppingList());

        _helper.ShoppingItemRepositoryMock
            .Setup(x => x.GetByShoppingListIdAsync(listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ShoppingItemEntity>());

        _helper.ItemQuoteRepositoryMock
            .Setup(x => x.GetByShoppingListIdAsync(listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ItemQuoteEntity>());

        var result = await _handler.ExecuteAsync(listId);

        Assert.NotNull(result);
        Assert.Equal(listId, result.ShoppingListId);
        Assert.Empty(result.Suppliers);
        Assert.Empty(result.Items);
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
