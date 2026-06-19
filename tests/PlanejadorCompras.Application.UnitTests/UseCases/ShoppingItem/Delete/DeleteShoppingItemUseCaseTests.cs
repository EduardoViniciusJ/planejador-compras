using Moq;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.UseCases.ShoppingItem;
using ShoppingItemEntity = PlanejadorCompras.Domain.Entities.ShoppingItem;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ShoppingItem.Delete;

public sealed class DeleteShoppingItemUseCaseTests
{
    private readonly DeleteShoppingItemTestHelper _helper;
    private readonly DeleteShoppingItemUseCase _handler;

    public DeleteShoppingItemUseCaseTests()
    {
        _helper = new DeleteShoppingItemTestHelper();
        _handler = new DeleteShoppingItemUseCase(
            _helper.ShoppingItemRepositoryMock.Object,
            _helper.UnitOfWorkMock.Object,
            _helper.ShoppingListAccessServiceMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldDeleteShoppingItem_WhenIdIsValid()
    {
        var shoppingItemId = DeleteShoppingItemTestHelper.DefaultShoppingItemId;
        var shoppingItem = ShoppingItemEntity.Create(shoppingItemId, "Monthly Tech Shopping Item", 2, "pcs");
        _helper.ShoppingItemRepositoryMock
            .Setup(x => x.GetByIdAsync(shoppingItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shoppingItem);
        _helper.SetupShoppingListAccess(shoppingItem.ShoppingListId);

        await _handler.ExecuteAsync(shoppingItemId);

        _helper.ShoppingItemRepositoryMock.Verify(
            x => x.DeleteAsync(
                shoppingItemId,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCommitUnitOfWork_WhenDeletionSucceeds()
    {
        var shoppingItemId = DeleteShoppingItemTestHelper.DefaultShoppingItemId;
        var shoppingItem = ShoppingItemEntity.Create(shoppingItemId, "Monthly Tech Shopping Item", 2, "pcs");
        _helper.ShoppingItemRepositoryMock
            .Setup(x => x.GetByIdAsync(shoppingItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shoppingItem);
        _helper.SetupShoppingListAccess(shoppingItem.ShoppingListId);

        await _handler.ExecuteAsync(shoppingItemId);

        _helper.UnitOfWorkMock.Verify(
            x => x.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowNotFoundException_WhenShoppingItemDoesNotExist()
    {
        var shoppingItemId = Guid.NewGuid();
        _helper.ShoppingItemRepositoryMock
            .Setup(x => x.GetByIdAsync(shoppingItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ShoppingItemEntity?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.ExecuteAsync(shoppingItemId));

        _helper.UnitOfWorkMock.Verify(
            x => x.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowArgumentOutOfRangeException_WhenIdIsEmpty()
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _handler.ExecuteAsync(Guid.Empty));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldNotCallRepositoryOrCommit_WhenIdIsEmpty()
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _handler.ExecuteAsync(Guid.Empty));

        _helper.ShoppingItemRepositoryMock.Verify(
            x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _helper.UnitOfWorkMock.Verify(
            x => x.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowNotFoundException_WhenListDoesNotBelongToUser()
    {
        var shoppingItem = ShoppingItemEntity.Create(DeleteShoppingItemTestHelper.DefaultShoppingItemId, "Monthly Tech Shopping Item", 2, "pcs");
        _helper.ShoppingItemRepositoryMock
            .Setup(x => x.GetByIdAsync(shoppingItem.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shoppingItem);
        _helper.ShoppingListAccessServiceMock
            .Setup(x => x.GetForCurrentUserAsync(shoppingItem.ShoppingListId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Shopping list not found.", "shopping_list_not_found"));

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.ExecuteAsync(shoppingItem.Id));
    }
}
