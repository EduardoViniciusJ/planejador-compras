using Moq;
using PlanejadorCompras.Application.UseCases.ItemQuote;
using ItemQuoteEntity = PlanejadorCompras.Domain.Entities.ItemQuote;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ItemQuote.Update;

public sealed class UpdateItemQuoteUseCaseTests
{
    private readonly UpdateItemQuoteTestHelper _helper;
    private readonly UpdateItemQuoteUseCase _handler;

    public UpdateItemQuoteUseCaseTests()
    {
        _helper = new UpdateItemQuoteTestHelper();
        _handler = new UpdateItemQuoteUseCase(
            _helper.ItemQuoteRepositoryMock.Object,
            _helper.UnitOfWorkMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldUpdateItemQuote_WhenRequestIsValid()
    {
        var itemQuote = ItemQuoteEntity.Create(UpdateItemQuoteTestHelper.DefaultShoppingItemId, "Supplier A", 199.90m);
        var request = UpdateItemQuoteTestHelper.CreateRequestDto(itemQuote.ShoppingItemId, "Updated Supplier", 175.50m);

        _helper.ItemQuoteRepositoryMock
            .Setup(x => x.GetByIdAsync(itemQuote.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(itemQuote);

        var response = await _handler.ExecuteAsync(itemQuote.Id, request);

        Assert.NotNull(response);
        Assert.Equal(itemQuote.Id, response.Id);
        Assert.Equal(request.ShoppingItemId, response.ShoppingItemId);
        Assert.Equal(request.SupplierName, response.SupplierName);
        Assert.Equal(request.UnitPrice, response.UnitPrice);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCallUpdateRepositoryWithCorrectData()
    {
        var itemQuote = ItemQuoteEntity.Create(UpdateItemQuoteTestHelper.DefaultShoppingItemId, "Supplier A", 199.90m);
        var request = UpdateItemQuoteTestHelper.CreateRequestDto(itemQuote.ShoppingItemId, "Updated Supplier", 175.50m);

        _helper.ItemQuoteRepositoryMock
            .Setup(x => x.GetByIdAsync(itemQuote.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(itemQuote);

        await _handler.ExecuteAsync(itemQuote.Id, request);

        _helper.ItemQuoteRepositoryMock.Verify(
            x => x.UpdateAsync(
                It.Is<ItemQuoteEntity>(iq =>
                    iq.Id == itemQuote.Id &&
                    iq.ShoppingItemId == request.ShoppingItemId &&
                    iq.SupplierName == request.SupplierName &&
                    iq.UnitPrice == request.UnitPrice),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCommitUnitOfWork_WhenUpdateSucceeds()
    {
        var itemQuote = ItemQuoteEntity.Create(UpdateItemQuoteTestHelper.DefaultShoppingItemId, "Supplier A", 199.90m);
        var request = UpdateItemQuoteTestHelper.CreateRequestDto(itemQuote.ShoppingItemId);

        _helper.ItemQuoteRepositoryMock
            .Setup(x => x.GetByIdAsync(itemQuote.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(itemQuote);

        await _handler.ExecuteAsync(itemQuote.Id, request);

        _helper.UnitOfWorkMock.Verify(
            x => x.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnNull_WhenItemQuoteDoesNotExist()
    {
        var itemQuoteId = Guid.NewGuid();
        var request = UpdateItemQuoteTestHelper.CreateRequestDto();

        _helper.ItemQuoteRepositoryMock
            .Setup(x => x.GetByIdAsync(itemQuoteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ItemQuoteEntity?)null);

        var response = await _handler.ExecuteAsync(itemQuoteId, request);

        Assert.Null(response);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.ExecuteAsync(Guid.NewGuid(), null!));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowArgumentOutOfRangeException_WhenIdIsEmpty()
    {
        var request = UpdateItemQuoteTestHelper.CreateRequestDto();

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _handler.ExecuteAsync(Guid.Empty, request));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldNotCallUpdateOrCommit_WhenItemQuoteDoesNotExist()
    {
        var itemQuoteId = Guid.NewGuid();
        var request = UpdateItemQuoteTestHelper.CreateRequestDto();

        _helper.ItemQuoteRepositoryMock
            .Setup(x => x.GetByIdAsync(itemQuoteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ItemQuoteEntity?)null);

        await _handler.ExecuteAsync(itemQuoteId, request);

        _helper.ItemQuoteRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<ItemQuoteEntity>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _helper.UnitOfWorkMock.Verify(
            x => x.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
