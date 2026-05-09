using Moq;
using PlanejadorCompras.Application.UseCases.ShoppingItem;

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
            _helper.UnitOfWorkMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldDeleteShoppingItem_WhenIdIsValid()
    {
        var shoppingItemId = DeleteShoppingItemTestHelper.DefaultShoppingItemId;

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

        await _handler.ExecuteAsync(shoppingItemId);

        _helper.UnitOfWorkMock.Verify(
            x => x.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Once);
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
            x => x.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _helper.UnitOfWorkMock.Verify(
            x => x.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
