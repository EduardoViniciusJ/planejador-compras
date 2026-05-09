using Moq;
using PlanejadorCompras.Application.UseCases.ShoppingList;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ShoppingList.GetById;

public sealed class GetShoppingListByIdUseCaseTests
{
    private readonly GetShoppingListByIdTestHelper _helper;
    private readonly GetShoppingListByIdUseCase _handler;

    public GetShoppingListByIdUseCaseTests()
    {
        _helper = new GetShoppingListByIdTestHelper();
        _handler = new GetShoppingListByIdUseCase(_helper.ShoppingListRepositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnShoppingList_WhenShoppingListExists()
    {
        var shoppingList = GetShoppingListByIdTestHelper.CreateShoppingListEntity();
        _helper.ShoppingListRepositoryMock
            .Setup(x => x.GetByIdAsync(shoppingList.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shoppingList);

        var response = await _handler.ExecuteAsync(shoppingList.Id);

        Assert.NotNull(response);
        Assert.Equal(shoppingList.Id, response.Id);
        Assert.Equal(shoppingList.UserId, response.UserId);
        Assert.Equal(shoppingList.Name, response.Name);
        Assert.Equal(shoppingList.Description, response.Description);
        Assert.Equal(shoppingList.CreatedAt, response.CreatedAt);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnNull_WhenShoppingListDoesNotExist()
    {
        var shoppingListId = Guid.NewGuid();
        _helper.ShoppingListRepositoryMock
            .Setup(x => x.GetByIdAsync(shoppingListId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PlanejadorCompras.Domain.Entities.ShoppingList?)null);

        var response = await _handler.ExecuteAsync(shoppingListId);

        Assert.Null(response);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowArgumentOutOfRangeException_WhenIdIsEmpty()
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _handler.ExecuteAsync(Guid.Empty));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCallRepositoryWithCorrectId()
    {
        var shoppingList = GetShoppingListByIdTestHelper.CreateShoppingListEntity();
        _helper.ShoppingListRepositoryMock
            .Setup(x => x.GetByIdAsync(shoppingList.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shoppingList);

        await _handler.ExecuteAsync(shoppingList.Id);

        _helper.ShoppingListRepositoryMock.Verify(
            x => x.GetByIdAsync(shoppingList.Id, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
