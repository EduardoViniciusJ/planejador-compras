using Moq;
using PlanejadorCompras.Application.UseCases.ShoppingList;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ShoppingList.GetByUserId;

public sealed class GetShoppingListsByUserIdUseCaseTests
{
    private readonly GetShoppingListsByUserIdTestHelper _helper;
    private readonly GetShoppingListsByUserIdUseCase _handler;

    public GetShoppingListsByUserIdUseCaseTests()
    {
        _helper = new GetShoppingListsByUserIdTestHelper();
        _handler = new GetShoppingListsByUserIdUseCase(_helper.ShoppingListRepositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnShoppingLists_WhenUserHasLists()
    {
        var userId = GetShoppingListsByUserIdTestHelper.DefaultUserId;
        var shoppingLists = new List<PlanejadorCompras.Domain.Entities.ShoppingList>
        {
            GetShoppingListsByUserIdTestHelper.CreateShoppingListEntity(userId, "Monthly Shopping List", "Monitor and printer ink"),
            GetShoppingListsByUserIdTestHelper.CreateShoppingListEntity(userId, "Office Setup List", "Keyboard, mouse, and webcam")
        };

        _helper.ShoppingListRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shoppingLists);

        var response = await _handler.ExecuteAsync(userId);

        Assert.Equal(2, response.Count);
        Assert.Equal(shoppingLists[0].Id, response[0].Id);
        Assert.Equal(shoppingLists[0].Name, response[0].Name);
        Assert.Equal(shoppingLists[1].Id, response[1].Id);
        Assert.Equal(shoppingLists[1].Name, response[1].Name);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnEmptyList_WhenUserHasNoLists()
    {
        var userId = GetShoppingListsByUserIdTestHelper.DefaultUserId;
        _helper.ShoppingListRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PlanejadorCompras.Domain.Entities.ShoppingList>());

        var response = await _handler.ExecuteAsync(userId);

        Assert.Empty(response);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowArgumentOutOfRangeException_WhenUserIdIsEmpty()
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _handler.ExecuteAsync(Guid.Empty));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCallRepositoryWithCorrectUserId()
    {
        var userId = GetShoppingListsByUserIdTestHelper.DefaultUserId;
        _helper.ShoppingListRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PlanejadorCompras.Domain.Entities.ShoppingList>());

        await _handler.ExecuteAsync(userId);

        _helper.ShoppingListRepositoryMock.Verify(
            x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
