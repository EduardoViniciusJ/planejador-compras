using Moq;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.UseCases.ItemQuote;
using ItemQuoteEntity = PlanejadorCompras.Domain.Entities.ItemQuote;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ItemQuote.GetById;

public sealed class GetItemQuoteByIdUseCaseTests
{
    private readonly GetItemQuoteByIdTestHelper _helper;
    private readonly GetItemQuoteByIdUseCase _handler;

    public GetItemQuoteByIdUseCaseTests()
    {
        _helper = new GetItemQuoteByIdTestHelper();
        _handler = new GetItemQuoteByIdUseCase(_helper.ItemQuoteRepositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnItemQuote_WhenItemQuoteExists()
    {
        var itemQuote = GetItemQuoteByIdTestHelper.CreateItemQuoteEntity();
        _helper.ItemQuoteRepositoryMock
            .Setup(x => x.GetByIdAsync(itemQuote.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(itemQuote);

        var response = await _handler.ExecuteAsync(itemQuote.Id);

        Assert.Equal(itemQuote.Id, response!.Id);
        Assert.Equal(itemQuote.ShoppingItemId, response.ShoppingItemId);
        Assert.Equal(itemQuote.SupplierName, response.SupplierName);
        Assert.Equal(itemQuote.UnitPrice, response.UnitPrice);
        Assert.Equal(itemQuote.CreatedAt, response.CreatedAt);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowNotFoundException_WhenItemQuoteDoesNotExist()
    {
        var itemQuoteId = Guid.NewGuid();
        _helper.ItemQuoteRepositoryMock
            .Setup(x => x.GetByIdAsync(itemQuoteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ItemQuoteEntity?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.ExecuteAsync(itemQuoteId));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowArgumentOutOfRangeException_WhenIdIsEmpty()
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _handler.ExecuteAsync(Guid.Empty));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCallRepositoryWithCorrectId()
    {
        var itemQuote = GetItemQuoteByIdTestHelper.CreateItemQuoteEntity();
        _helper.ItemQuoteRepositoryMock
            .Setup(x => x.GetByIdAsync(itemQuote.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(itemQuote);

        await _handler.ExecuteAsync(itemQuote.Id);

        _helper.ItemQuoteRepositoryMock.Verify(
            x => x.GetByIdAsync(itemQuote.Id, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
