using Moq;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.UseCases.ShoppingItem;
using ShoppingItemEntity = PlanejadorCompras.Domain.Entities.ShoppingItem;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ShoppingItem.Update;

public sealed class UpdateShoppingItemUseCaseTests
{
    private readonly UpdateShoppingItemTestHelper _helper;
    private readonly UpdateShoppingItemUseCase _handler;

    public UpdateShoppingItemUseCaseTests()
    {
        _helper = new UpdateShoppingItemTestHelper();
        _handler = new UpdateShoppingItemUseCase(
            _helper.ShoppingItemRepositoryMock.Object,
            _helper.UnitOfWorkMock.Object,
            _helper.ShoppingListAccessServiceMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldUpdateShoppingItem_WhenRequestIsValid()
    {
        var shoppingItem = UpdateShoppingItemTestHelper.CreateShoppingItemEntity();
        var request = UpdateShoppingItemTestHelper.CreateRequestDto(
            shoppingItem.ShoppingListId,
            "Updated Tech Shopping Item",
            5,
            "box");
        _helper.SetupShoppingListAccess(shoppingItem.ShoppingListId);
        _helper.SetupShoppingListAccess(request.ShoppingListId);

        _helper.ShoppingItemRepositoryMock
            .Setup(x => x.GetByIdAsync(shoppingItem.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shoppingItem);

        var response = await _handler.ExecuteAsync(shoppingItem.Id, request);

        Assert.NotNull(response);
        Assert.Equal(shoppingItem.Id, response.Id);
        Assert.Equal(request.ShoppingListId, response.ShoppingListId);
        Assert.Equal(request.Name, response.Name);
        Assert.Equal(request.Quantity, response.Quantity);
        Assert.Equal(request.Unit, response.Unit);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCallUpdateRepositoryWithCorrectData()
    {
        var shoppingItem = UpdateShoppingItemTestHelper.CreateShoppingItemEntity();
        var request = UpdateShoppingItemTestHelper.CreateRequestDto(
            shoppingItem.ShoppingListId,
            "Office Upgrade Item",
            4,
            "pcs");
        _helper.SetupShoppingListAccess(shoppingItem.ShoppingListId);
        _helper.SetupShoppingListAccess(request.ShoppingListId);

        _helper.ShoppingItemRepositoryMock
            .Setup(x => x.GetByIdAsync(shoppingItem.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shoppingItem);

        await _handler.ExecuteAsync(shoppingItem.Id, request);

        _helper.ShoppingItemRepositoryMock.Verify(
            x => x.UpdateAsync(
                It.Is<ShoppingItemEntity>(s =>
                    s.Id == shoppingItem.Id &&
                    s.ShoppingListId == request.ShoppingListId &&
                    s.Name == request.Name &&
                    s.Quantity == request.Quantity &&
                    s.Unit == request.Unit),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCommitUnitOfWork_WhenUpdateSucceeds()
    {
        var shoppingItem = UpdateShoppingItemTestHelper.CreateShoppingItemEntity();
        var request = UpdateShoppingItemTestHelper.CreateRequestDto(shoppingItem.ShoppingListId);
        _helper.SetupShoppingListAccess(shoppingItem.ShoppingListId);
        _helper.SetupShoppingListAccess(request.ShoppingListId);

        _helper.ShoppingItemRepositoryMock
            .Setup(x => x.GetByIdAsync(shoppingItem.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shoppingItem);

        await _handler.ExecuteAsync(shoppingItem.Id, request);

        _helper.UnitOfWorkMock.Verify(
            x => x.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowNotFoundException_WhenShoppingItemDoesNotExist()
    {
        var shoppingItemId = Guid.NewGuid();
        var request = UpdateShoppingItemTestHelper.CreateRequestDto();

        _helper.ShoppingItemRepositoryMock
            .Setup(x => x.GetByIdAsync(shoppingItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ShoppingItemEntity?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.ExecuteAsync(shoppingItemId, request));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.ExecuteAsync(Guid.NewGuid(), null!));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowArgumentOutOfRangeException_WhenIdIsEmpty()
    {
        var request = UpdateShoppingItemTestHelper.CreateRequestDto();

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _handler.ExecuteAsync(Guid.Empty, request));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldNotCallUpdateOrCommit_WhenShoppingItemDoesNotExist()
    {
        var shoppingItemId = Guid.NewGuid();
        var request = UpdateShoppingItemTestHelper.CreateRequestDto();

        _helper.ShoppingItemRepositoryMock
            .Setup(x => x.GetByIdAsync(shoppingItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ShoppingItemEntity?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.ExecuteAsync(shoppingItemId, request));

        _helper.ShoppingItemRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<ShoppingItemEntity>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _helper.UnitOfWorkMock.Verify(
            x => x.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowNotFoundException_WhenTargetListDoesNotBelongToUser()
    {
        var shoppingItem = UpdateShoppingItemTestHelper.CreateShoppingItemEntity();
        var unauthorizedListId = Guid.NewGuid();
        var request = UpdateShoppingItemTestHelper.CreateRequestDto(unauthorizedListId);

        _helper.SetupShoppingListAccess(shoppingItem.ShoppingListId);
        _helper.ShoppingItemRepositoryMock
            .Setup(x => x.GetByIdAsync(shoppingItem.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shoppingItem);
        _helper.ShoppingListAccessServiceMock
            .Setup(x => x.GetForCurrentUserAsync(unauthorizedListId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Shopping list not found.", "shopping_list_not_found"));

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.ExecuteAsync(shoppingItem.Id, request));
    }
}
