using Moq;
using PlanejadorCompras.Application.UseCases.ItemQuote;
using ItemQuoteEntity = PlanejadorCompras.Domain.Entities.ItemQuote;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ItemQuote.GetByShoppingItemId;

public sealed class GetItemQuotesByShoppingItemIdUseCaseTests
{
    private readonly GetItemQuotesByShoppingItemIdTestHelper _helper;
    private readonly GetItemQuotesByShoppingItemIdUseCase _handler;

    public GetItemQuotesByShoppingItemIdUseCaseTests()
    {
        _helper = new GetItemQuotesByShoppingItemIdTestHelper();
        _handler = new GetItemQuotesByShoppingItemIdUseCase(_helper.ItemQuoteRepositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnItemQuotes_WhenShoppingItemHasQuotes()
    {
        var shoppingItemId = GetItemQuotesByShoppingItemIdTestHelper.DefaultShoppingItemId;
        var itemQuotes = new List<ItemQuoteEntity>
        {
            GetItemQuotesByShoppingItemIdTestHelper.CreateItemQuoteEntity(shoppingItemId, "Supplier A", 199.90m),
            GetItemQuotesByShoppingItemIdTestHelper.CreateItemQuoteEntity(shoppingItemId, "Supplier B", 189.90m)
        };

        _helper.ItemQuoteRepositoryMock
            .Setup(x => x.GetByShoppingItemIdAsync(shoppingItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(itemQuotes);

        var response = await _handler.ExecuteAsync(shoppingItemId);

        Assert.Equal(2, response.Count);
        Assert.Equal(itemQuotes[0].Id, response[0].Id);
        Assert.Equal(itemQuotes[0].SupplierName, response[0].SupplierName);
        Assert.Equal(itemQuotes[1].Id, response[1].Id);
        Assert.Equal(itemQuotes[1].SupplierName, response[1].SupplierName);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnEmptyList_WhenShoppingItemHasNoQuotes()
    {
        var shoppingItemId = GetItemQuotesByShoppingItemIdTestHelper.DefaultShoppingItemId;
        _helper.ItemQuoteRepositoryMock
            .Setup(x => x.GetByShoppingItemIdAsync(shoppingItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ItemQuoteEntity>());

        var response = await _handler.ExecuteAsync(shoppingItemId);

        Assert.Empty(response);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowArgumentOutOfRangeException_WhenShoppingItemIdIsEmpty()
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _handler.ExecuteAsync(Guid.Empty));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCallRepositoryWithCorrectShoppingItemId()
    {
        var shoppingItemId = GetItemQuotesByShoppingItemIdTestHelper.DefaultShoppingItemId;
        _helper.ItemQuoteRepositoryMock
            .Setup(x => x.GetByShoppingItemIdAsync(shoppingItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ItemQuoteEntity>());

        await _handler.ExecuteAsync(shoppingItemId);

        _helper.ItemQuoteRepositoryMock.Verify(
            x => x.GetByShoppingItemIdAsync(shoppingItemId, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
