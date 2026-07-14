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
        _handler = new GetItemQuotesByShoppingItemIdUseCase(
            _helper.ItemQuoteRepositoryMock.Object,
            _helper.ShoppingItemRepositoryMock.Object,
            _helper.ShoppingListAccessServiceMock.Object,
            _helper.SupplierRepositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnItemQuotes_WhenShoppingItemHasQuotes()
    {
        var shoppingItemId = GetItemQuotesByShoppingItemIdTestHelper.DefaultShoppingItemId;
        var shoppingItem = GetItemQuotesByShoppingItemIdTestHelper.CreateShoppingItemEntity();
        var itemQuotes = new List<ItemQuoteEntity>
        {
            GetItemQuotesByShoppingItemIdTestHelper.CreateItemQuoteEntity(
                shoppingItemId,
                GetItemQuotesByShoppingItemIdTestHelper.SupplierA.Id,
                199.90m),
            GetItemQuotesByShoppingItemIdTestHelper.CreateItemQuoteEntity(
                shoppingItemId,
                GetItemQuotesByShoppingItemIdTestHelper.SupplierB.Id,
                189.90m)
        };
        _helper.ShoppingItemRepositoryMock
            .Setup(x => x.GetByIdAsync(shoppingItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shoppingItem);
        _helper.ShoppingListAccessServiceMock
            .Setup(x => x.GetForCurrentUserAsync(shoppingItem.ShoppingListId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(PlanejadorCompras.Domain.Entities.ShoppingList.Create(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Authorized List"));

        _helper.ItemQuoteRepositoryMock
            .Setup(x => x.GetByShoppingItemIdAsync(shoppingItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(itemQuotes);

        var response = await _handler.ExecuteAsync(shoppingItemId);

        Assert.Equal(2, response.Count);
        Assert.Equal(itemQuotes[0].Id, response[0].Id);
        Assert.Equal(GetItemQuotesByShoppingItemIdTestHelper.SupplierA.Name, response[0].SupplierName);
        Assert.Equal(itemQuotes[1].Id, response[1].Id);
        Assert.Equal(GetItemQuotesByShoppingItemIdTestHelper.SupplierB.Name, response[1].SupplierName);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnEmptyList_WhenShoppingItemHasNoQuotes()
    {
        var shoppingItemId = GetItemQuotesByShoppingItemIdTestHelper.DefaultShoppingItemId;
        var shoppingItem = GetItemQuotesByShoppingItemIdTestHelper.CreateShoppingItemEntity();
        _helper.ShoppingItemRepositoryMock
            .Setup(x => x.GetByIdAsync(shoppingItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shoppingItem);
        _helper.ShoppingListAccessServiceMock
            .Setup(x => x.GetForCurrentUserAsync(shoppingItem.ShoppingListId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(PlanejadorCompras.Domain.Entities.ShoppingList.Create(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Authorized List"));
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
        var shoppingItem = GetItemQuotesByShoppingItemIdTestHelper.CreateShoppingItemEntity();
        _helper.ShoppingItemRepositoryMock
            .Setup(x => x.GetByIdAsync(shoppingItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shoppingItem);
        _helper.ShoppingListAccessServiceMock
            .Setup(x => x.GetForCurrentUserAsync(shoppingItem.ShoppingListId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(PlanejadorCompras.Domain.Entities.ShoppingList.Create(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Authorized List"));
        _helper.ItemQuoteRepositoryMock
            .Setup(x => x.GetByShoppingItemIdAsync(shoppingItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ItemQuoteEntity>());

        await _handler.ExecuteAsync(shoppingItemId);

        _helper.ItemQuoteRepositoryMock.Verify(
            x => x.GetByShoppingItemIdAsync(shoppingItemId, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
