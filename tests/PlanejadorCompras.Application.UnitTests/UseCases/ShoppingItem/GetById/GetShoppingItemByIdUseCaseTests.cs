using Moq;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.UseCases.ShoppingItem;
using ShoppingItemEntity = PlanejadorCompras.Domain.Entities.ShoppingItem;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ShoppingItem.GetById;

public sealed class GetShoppingItemByIdUseCaseTests
{
    private readonly GetShoppingItemByIdTestHelper _helper;
    private readonly GetShoppingItemByIdUseCase _handler;

    public GetShoppingItemByIdUseCaseTests()
    {
        _helper = new GetShoppingItemByIdTestHelper();
        _handler = new GetShoppingItemByIdUseCase(_helper.ShoppingItemRepositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnShoppingItem_WhenShoppingItemExists()
    {
        var shoppingItem = GetShoppingItemByIdTestHelper.CreateShoppingItemEntity();
        _helper.ShoppingItemRepositoryMock
            .Setup(x => x.GetByIdAsync(shoppingItem.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shoppingItem);

        var response = await _handler.ExecuteAsync(shoppingItem.Id);

        Assert.Equal(shoppingItem.Id, response!.Id);
        Assert.Equal(shoppingItem.ShoppingListId, response.ShoppingListId);
        Assert.Equal(shoppingItem.Name, response.Name);
        Assert.Equal(shoppingItem.Quantity, response.Quantity);
        Assert.Equal(shoppingItem.Unit, response.Unit);
        Assert.Equal(shoppingItem.CreatedAt, response.CreatedAt);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowNotFoundException_WhenShoppingItemDoesNotExist()
    {
        var shoppingItemId = Guid.NewGuid();
        _helper.ShoppingItemRepositoryMock
            .Setup(x => x.GetByIdAsync(shoppingItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ShoppingItemEntity?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.ExecuteAsync(shoppingItemId));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowArgumentOutOfRangeException_WhenIdIsEmpty()
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _handler.ExecuteAsync(Guid.Empty));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCallRepositoryWithCorrectId()
    {
        var shoppingItem = GetShoppingItemByIdTestHelper.CreateShoppingItemEntity();
        _helper.ShoppingItemRepositoryMock
            .Setup(x => x.GetByIdAsync(shoppingItem.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shoppingItem);

        await _handler.ExecuteAsync(shoppingItem.Id);

        _helper.ShoppingItemRepositoryMock.Verify(
            x => x.GetByIdAsync(shoppingItem.Id, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
