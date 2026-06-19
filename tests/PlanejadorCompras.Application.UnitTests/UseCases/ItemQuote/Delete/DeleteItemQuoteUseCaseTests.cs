using Moq;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.UseCases.ItemQuote;
using ItemQuoteEntity = PlanejadorCompras.Domain.Entities.ItemQuote;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ItemQuote.Delete;

public sealed class DeleteItemQuoteUseCaseTests
{
    private readonly DeleteItemQuoteTestHelper _helper;
    private readonly DeleteItemQuoteUseCase _handler;

    public DeleteItemQuoteUseCaseTests()
    {
        _helper = new DeleteItemQuoteTestHelper();
        _handler = new DeleteItemQuoteUseCase(
            _helper.ItemQuoteRepositoryMock.Object,
            _helper.ShoppingItemRepositoryMock.Object,
            _helper.UnitOfWorkMock.Object,
            _helper.ShoppingListAccessServiceMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldDeleteItemQuote_WhenIdIsValid()
    {
        var itemQuoteId = DeleteItemQuoteTestHelper.DefaultItemQuoteId;
        var itemQuote = ItemQuoteEntity.Create(DeleteItemQuoteTestHelper.CreateShoppingItemEntity().Id, "Supplier A", 199.90m);
        var shoppingItem = DeleteItemQuoteTestHelper.CreateShoppingItemEntity();
        _helper.ItemQuoteRepositoryMock
            .Setup(x => x.GetByIdAsync(itemQuoteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(itemQuote);
        _helper.ShoppingItemRepositoryMock
            .Setup(x => x.GetByIdAsync(itemQuote.ShoppingItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shoppingItem);
        _helper.ShoppingListAccessServiceMock
            .Setup(x => x.GetForCurrentUserAsync(shoppingItem.ShoppingListId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(PlanejadorCompras.Domain.Entities.ShoppingList.Create(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Authorized List"));

        await _handler.ExecuteAsync(itemQuoteId);

        _helper.ItemQuoteRepositoryMock.Verify(
            x => x.DeleteAsync(itemQuoteId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCommitUnitOfWork_WhenDeletionSucceeds()
    {
        var itemQuoteId = DeleteItemQuoteTestHelper.DefaultItemQuoteId;
        var shoppingItem = DeleteItemQuoteTestHelper.CreateShoppingItemEntity();
        var itemQuote = ItemQuoteEntity.Create(shoppingItem.Id, "Supplier A", 199.90m);
        _helper.ItemQuoteRepositoryMock
            .Setup(x => x.GetByIdAsync(itemQuoteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(itemQuote);
        _helper.ShoppingItemRepositoryMock
            .Setup(x => x.GetByIdAsync(itemQuote.ShoppingItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shoppingItem);
        _helper.ShoppingListAccessServiceMock
            .Setup(x => x.GetForCurrentUserAsync(shoppingItem.ShoppingListId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(PlanejadorCompras.Domain.Entities.ShoppingList.Create(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Authorized List"));

        await _handler.ExecuteAsync(itemQuoteId);

        _helper.UnitOfWorkMock.Verify(
            x => x.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowNotFoundException_WhenItemQuoteDoesNotExist()
    {
        var itemQuoteId = Guid.NewGuid();
        _helper.ItemQuoteRepositoryMock
            .Setup(x => x.GetByIdAsync(itemQuoteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ItemQuoteEntity?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.ExecuteAsync(itemQuoteId));

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

        _helper.ItemQuoteRepositoryMock.Verify(
            x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _helper.UnitOfWorkMock.Verify(
            x => x.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowNotFoundException_WhenListDoesNotBelongToUser()
    {
        var shoppingItem = DeleteItemQuoteTestHelper.CreateShoppingItemEntity();
        var itemQuote = ItemQuoteEntity.Create(shoppingItem.Id, "Supplier A", 199.90m);
        _helper.ItemQuoteRepositoryMock
            .Setup(x => x.GetByIdAsync(itemQuote.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(itemQuote);
        _helper.ShoppingItemRepositoryMock
            .Setup(x => x.GetByIdAsync(itemQuote.ShoppingItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shoppingItem);
        _helper.ShoppingListAccessServiceMock
            .Setup(x => x.GetForCurrentUserAsync(shoppingItem.ShoppingListId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Shopping list not found.", "shopping_list_not_found"));

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.ExecuteAsync(itemQuote.Id));
    }
}
