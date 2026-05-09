using Moq;
using PlanejadorCompras.Application.UseCases.ShoppingItem;
using ShoppingItemEntity = PlanejadorCompras.Domain.Entities.ShoppingItem;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ShoppingItem.GetByShoppingListId;

public sealed class GetShoppingItemsByShoppingListIdUseCaseTests
{
    private readonly GetShoppingItemsByShoppingListIdTestHelper _helper;
    private readonly GetShoppingItemsByShoppingListIdUseCase _handler;

    public GetShoppingItemsByShoppingListIdUseCaseTests()
    {
        _helper = new GetShoppingItemsByShoppingListIdTestHelper();
        _handler = new GetShoppingItemsByShoppingListIdUseCase(_helper.ShoppingItemRepositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnShoppingItems_WhenListHasItems()
    {
        var shoppingListId = GetShoppingItemsByShoppingListIdTestHelper.DefaultShoppingListId;
        var shoppingItems = new List<ShoppingItemEntity>
        {
            GetShoppingItemsByShoppingListIdTestHelper.CreateShoppingItemEntity(shoppingListId, "Monitor", 1, "pcs"),
            GetShoppingItemsByShoppingListIdTestHelper.CreateShoppingItemEntity(shoppingListId, "Keyboard", 2, "pcs")
        };

        _helper.ShoppingItemRepositoryMock
            .Setup(x => x.GetByShoppingListIdAsync(shoppingListId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shoppingItems);

        var response = await _handler.ExecuteAsync(shoppingListId);

        Assert.Equal(2, response.Count);
        Assert.Equal(shoppingItems[0].Id, response[0].Id);
        Assert.Equal(shoppingItems[0].Name, response[0].Name);
        Assert.Equal(shoppingItems[1].Id, response[1].Id);
        Assert.Equal(shoppingItems[1].Name, response[1].Name);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnEmptyList_WhenListHasNoItems()
    {
        var shoppingListId = GetShoppingItemsByShoppingListIdTestHelper.DefaultShoppingListId;
        _helper.ShoppingItemRepositoryMock
            .Setup(x => x.GetByShoppingListIdAsync(shoppingListId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ShoppingItemEntity>());

        var response = await _handler.ExecuteAsync(shoppingListId);

        Assert.Empty(response);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowArgumentOutOfRangeException_WhenShoppingListIdIsEmpty()
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _handler.ExecuteAsync(Guid.Empty));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCallRepositoryWithCorrectShoppingListId()
    {
        var shoppingListId = GetShoppingItemsByShoppingListIdTestHelper.DefaultShoppingListId;
        _helper.ShoppingItemRepositoryMock
            .Setup(x => x.GetByShoppingListIdAsync(shoppingListId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ShoppingItemEntity>());

        await _handler.ExecuteAsync(shoppingListId);

        _helper.ShoppingItemRepositoryMock.Verify(
            x => x.GetByShoppingListIdAsync(shoppingListId, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
