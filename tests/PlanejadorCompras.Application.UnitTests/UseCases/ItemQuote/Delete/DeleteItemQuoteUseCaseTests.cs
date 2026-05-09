using Moq;
using PlanejadorCompras.Application.UseCases.ItemQuote;

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
            _helper.UnitOfWorkMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldDeleteItemQuote_WhenIdIsValid()
    {
        var itemQuoteId = DeleteItemQuoteTestHelper.DefaultItemQuoteId;

        await _handler.ExecuteAsync(itemQuoteId);

        _helper.ItemQuoteRepositoryMock.Verify(
            x => x.DeleteAsync(itemQuoteId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCommitUnitOfWork_WhenDeletionSucceeds()
    {
        var itemQuoteId = DeleteItemQuoteTestHelper.DefaultItemQuoteId;

        await _handler.ExecuteAsync(itemQuoteId);

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

        _helper.ItemQuoteRepositoryMock.Verify(
            x => x.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _helper.UnitOfWorkMock.Verify(
            x => x.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
