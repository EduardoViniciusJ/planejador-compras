using Moq;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.UseCases.ShoppingList;
using PlanejadorCompras.Infrastructure.Services;
using ShoppingListEntity = PlanejadorCompras.Domain.Entities.ShoppingList;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ShoppingList.Update;

public sealed class UpdateShoppingListUseCaseTests
{
    private readonly UpdateShoppingListTestHelper _helper;
    private readonly UpdateShoppingListUseCase _handler;

    public UpdateShoppingListUseCaseTests()
    {
        _helper = new UpdateShoppingListTestHelper();
        var accessService = new ShoppingListAccessService(
            _helper.ShoppingListRepositoryMock.Object,
            _helper.CurrentUserMock.Object);

        _handler = new UpdateShoppingListUseCase(
            _helper.ShoppingListRepositoryMock.Object,
            _helper.UnitOfWorkMock.Object,
            accessService);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldUpdateShoppingList_WhenRequestIsValid()
    {
        var shoppingList = UpdateShoppingListTestHelper.CreateShoppingListEntity();
        var request = UpdateShoppingListTestHelper.CreateRequestDto("Updated Monthly Shopping", "Monitor arm and docking station");
        _helper.ShoppingListRepositoryMock
            .Setup(x => x.GetByIdAsync(shoppingList.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shoppingList);

        var response = await _handler.ExecuteAsync(shoppingList.Id, request);

        Assert.NotNull(response);
        Assert.Equal(shoppingList.Id, response.Id);
        Assert.Equal(request.Name, response.Name);
        Assert.Equal(request.Description, response.Description);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCallUpdateRepositoryWithCorrectData()
    {
        var shoppingList = UpdateShoppingListTestHelper.CreateShoppingListEntity();
        var request = UpdateShoppingListTestHelper.CreateRequestDto("Office Setup Refresh", "Webcam and monitor light bar");
        _helper.ShoppingListRepositoryMock
            .Setup(x => x.GetByIdAsync(shoppingList.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shoppingList);

        await _handler.ExecuteAsync(shoppingList.Id, request);

        _helper.ShoppingListRepositoryMock.Verify(
            x => x.UpdateAsync(
                It.Is<ShoppingListEntity>(s =>
                    s.Id == shoppingList.Id &&
                    s.UserId == shoppingList.UserId &&
                    s.Name == request.Name &&
                    s.Description == request.Description),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCommitUnitOfWork_WhenUpdateSucceeds()
    {
        var shoppingList = UpdateShoppingListTestHelper.CreateShoppingListEntity();
        var request = UpdateShoppingListTestHelper.CreateRequestDto();
        _helper.ShoppingListRepositoryMock
            .Setup(x => x.GetByIdAsync(shoppingList.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shoppingList);

        await _handler.ExecuteAsync(shoppingList.Id, request);

        _helper.UnitOfWorkMock.Verify(
            x => x.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowNotFoundException_WhenShoppingListDoesNotExist()
    {
        var shoppingListId = Guid.NewGuid();
        var request = UpdateShoppingListTestHelper.CreateRequestDto();
        _helper.ShoppingListRepositoryMock
            .Setup(x => x.GetByIdAsync(shoppingListId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ShoppingListEntity?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.ExecuteAsync(shoppingListId, request));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowNotFoundException_WhenShoppingListDoesNotBelongToUser()
    {
        var shoppingList = UpdateShoppingListTestHelper.CreateShoppingListEntity(Guid.NewGuid());
        var request = UpdateShoppingListTestHelper.CreateRequestDto();
        _helper.ShoppingListRepositoryMock
            .Setup(x => x.GetByIdAsync(shoppingList.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shoppingList);

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.ExecuteAsync(shoppingList.Id, request));

        _helper.ShoppingListRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<ShoppingListEntity>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.ExecuteAsync(Guid.NewGuid(), null!));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowArgumentOutOfRangeException_WhenIdIsEmpty()
    {
        var request = UpdateShoppingListTestHelper.CreateRequestDto();

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _handler.ExecuteAsync(Guid.Empty, request));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldNotCallUpdateOrCommit_WhenShoppingListDoesNotExist()
    {
        var shoppingListId = Guid.NewGuid();
        var request = UpdateShoppingListTestHelper.CreateRequestDto();
        _helper.ShoppingListRepositoryMock
            .Setup(x => x.GetByIdAsync(shoppingListId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ShoppingListEntity?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.ExecuteAsync(shoppingListId, request));

        _helper.ShoppingListRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<ShoppingListEntity>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _helper.UnitOfWorkMock.Verify(
            x => x.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
