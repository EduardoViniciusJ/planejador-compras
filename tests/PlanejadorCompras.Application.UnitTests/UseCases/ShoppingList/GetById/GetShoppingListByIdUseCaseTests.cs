using Moq;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.UseCases.ShoppingList;
using PlanejadorCompras.Infrastructure.Services;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ShoppingList.GetById;

public sealed class GetShoppingListByIdUseCaseTests
{
    private readonly GetShoppingListByIdTestHelper _helper;
    private readonly GetShoppingListByIdUseCase _handler;

    public GetShoppingListByIdUseCaseTests()
    {
        _helper = new GetShoppingListByIdTestHelper();
        var accessService = new ShoppingListAccessService(
            _helper.ShoppingListRepositoryMock.Object,
            _helper.CurrentUserMock.Object);

        _handler = new GetShoppingListByIdUseCase(accessService);
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
        Assert.Equal(shoppingList.Name, response.Name);
        Assert.Equal(shoppingList.Description, response.Description);
        Assert.Equal(shoppingList.CreatedAt, response.CreatedAt);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowNotFoundException_WhenShoppingListDoesNotExist()
    {
        var shoppingListId = Guid.NewGuid();
        _helper.ShoppingListRepositoryMock
            .Setup(x => x.GetByIdAsync(shoppingListId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PlanejadorCompras.Domain.Entities.ShoppingList?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.ExecuteAsync(shoppingListId));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowNotFoundException_WhenShoppingListDoesNotBelongToUser()
    {
        var otherUserList = GetShoppingListByIdTestHelper.CreateShoppingListEntity(Guid.NewGuid());
        _helper.ShoppingListRepositoryMock
            .Setup(x => x.GetByIdAsync(otherUserList.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(otherUserList);

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.ExecuteAsync(otherUserList.Id));
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
