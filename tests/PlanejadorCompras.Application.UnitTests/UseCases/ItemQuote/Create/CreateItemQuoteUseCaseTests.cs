using Moq;
using PlanejadorCompras.Application.Common.Dtos.Requests;
using PlanejadorCompras.Application.UseCases.ItemQuote.Create;
using ItemQuoteEntity = PlanejadorCompras.Domain.Entities.ItemQuote;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ItemQuote.Create;

public sealed class CreateItemQuoteUseCaseTests
{
    private readonly CreateItemQuoteTestHelper _helper;
    private readonly CreateItemQuoteUseCase _handler;

    public CreateItemQuoteUseCaseTests()
    {
        _helper = new CreateItemQuoteTestHelper();
        _handler = new CreateItemQuoteUseCase(
            _helper.ItemQuoteRepositoryMock.Object,
            _helper.UnitOfWorkMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCreateItemQuote_WhenRequestIsValid()
    {
        var request = new ItemQuoteRequestDto(
            CreateItemQuoteTestHelper.DefaultShoppingItemId,
            "Best Monitor Supplier",
            199.90m);

        var response = await _handler.ExecuteAsync(request);

        Assert.Equal(request.ShoppingItemId, response.ShoppingItemId);
        Assert.Equal(request.SupplierName, response.SupplierName);
        Assert.Equal(request.UnitPrice, response.UnitPrice);
        Assert.NotEqual(Guid.Empty, response.Id);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCallRepositoryWithCorrectData()
    {
        var request = CreateItemQuoteTestHelper.CreateRequestDto();

        await _handler.ExecuteAsync(request);

        _helper.ItemQuoteRepositoryMock.Verify(
            x => x.AddAsync(
                It.Is<ItemQuoteEntity>(iq =>
                    iq.ShoppingItemId == request.ShoppingItemId &&
                    iq.SupplierName == request.SupplierName &&
                    iq.UnitPrice == request.UnitPrice),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCommitUnitOfWork_WhenCreationSucceeds()
    {
        var request = CreateItemQuoteTestHelper.CreateRequestDto();

        await _handler.ExecuteAsync(request);

        _helper.UnitOfWorkMock.Verify(
            x => x.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.ExecuteAsync(null!));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldTrimSupplierName()
    {
        var request = new ItemQuoteRequestDto(
            CreateItemQuoteTestHelper.DefaultShoppingItemId,
            "  Best Monitor Supplier  ",
            199.90m);

        var response = await _handler.ExecuteAsync(request);

        Assert.Equal("Best Monitor Supplier", response.SupplierName);
    }
}
