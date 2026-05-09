using Moq;
using PlanejadorCompras.Application.UseCases.ShoppingList;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ShoppingList.Delete;

public sealed class DeleteShoppingListUseCaseTests
{
    private readonly DeleteShoppingListTestHelper _helper;
    private readonly DeleteShoppingListUseCase _handler;

    public DeleteShoppingListUseCaseTests()
    {
        _helper = new DeleteShoppingListTestHelper();
        _handler = new DeleteShoppingListUseCase(
            _helper.ShoppingListRepositoryMock.Object,
            _helper.UnitOfWorkMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldDeleteShoppingList_WhenIdIsValid()
    {
        var shoppingListId = DeleteShoppingListTestHelper.DefaultShoppingListId;

        await _handler.ExecuteAsync(shoppingListId);

        _helper.ShoppingListRepositoryMock.Verify(
            x => x.DeleteAsync(
                shoppingListId,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCommitUnitOfWork_WhenDeletionSucceeds()
    {
        var shoppingListId = DeleteShoppingListTestHelper.DefaultShoppingListId;

        await _handler.ExecuteAsync(shoppingListId);

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

        _helper.ShoppingListRepositoryMock.Verify(
            x => x.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _helper.UnitOfWorkMock.Verify(
            x => x.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
