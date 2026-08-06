using PlanejadorCompras.Application.Features.ItemQuotes.Contracts;
using Moq;
using PlanejadorCompras.Application.Exceptions;
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
            _helper.ShoppingItemRepositoryMock.Object,
            _helper.UnitOfWorkMock.Object,
            _helper.ShoppingListAccessServiceMock.Object,
            _helper.SupplierAccessServiceMock.Object,
            _helper.ShoppingListSupplierRepositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCreateItemQuote_WhenRequestIsValid()
    {
        var request = new ItemQuoteRequestDto(
            CreateItemQuoteTestHelper.DefaultShoppingItemId,
            CreateItemQuoteTestHelper.DefaultSupplier.Id,
            199.90m);
        var shoppingItem = CreateItemQuoteTestHelper.CreateShoppingItemEntity();
        _helper.ShoppingItemRepositoryMock
            .Setup(x => x.GetByIdAsync(request.ShoppingItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shoppingItem);
        _helper.SetupShoppingListAccess(shoppingItem.ShoppingListId);

        var response = await _handler.ExecuteAsync(request);

        Assert.Equal(request.ShoppingItemId, response.ShoppingItemId);
        Assert.Equal(request.SupplierId, response.SupplierId);
        Assert.Equal(CreateItemQuoteTestHelper.DefaultSupplier.Name, response.SupplierName);
        Assert.Equal(request.UnitPrice, response.UnitPrice);
        Assert.NotEqual(Guid.Empty, response.Id);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCallRepositoryWithCorrectData()
    {
        var request = CreateItemQuoteTestHelper.CreateRequestDto();
        var shoppingItem = CreateItemQuoteTestHelper.CreateShoppingItemEntity();
        _helper.ShoppingItemRepositoryMock
            .Setup(x => x.GetByIdAsync(request.ShoppingItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shoppingItem);
        _helper.SetupShoppingListAccess(shoppingItem.ShoppingListId);

        await _handler.ExecuteAsync(request);

        _helper.ItemQuoteRepositoryMock.Verify(
            x => x.AddAsync(
                It.Is<ItemQuoteEntity>(iq =>
                    iq.ShoppingItemId == request.ShoppingItemId &&
                    iq.SupplierId == request.SupplierId &&
                    iq.UnitPrice == request.UnitPrice),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCommitUnitOfWork_WhenCreationSucceeds()
    {
        var request = CreateItemQuoteTestHelper.CreateRequestDto();
        var shoppingItem = CreateItemQuoteTestHelper.CreateShoppingItemEntity();
        _helper.ShoppingItemRepositoryMock
            .Setup(x => x.GetByIdAsync(request.ShoppingItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shoppingItem);
        _helper.SetupShoppingListAccess(shoppingItem.ShoppingListId);

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
    public async Task ExecuteAsync_ShouldReturnRegisteredSupplierName()
    {
        var request = new ItemQuoteRequestDto(
            CreateItemQuoteTestHelper.DefaultShoppingItemId,
            CreateItemQuoteTestHelper.DefaultSupplier.Id,
            199.90m);
        var shoppingItem = CreateItemQuoteTestHelper.CreateShoppingItemEntity();
        _helper.ShoppingItemRepositoryMock
            .Setup(x => x.GetByIdAsync(request.ShoppingItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shoppingItem);
        _helper.SetupShoppingListAccess(shoppingItem.ShoppingListId);

        var response = await _handler.ExecuteAsync(request);

        Assert.Equal("Best Monitor Supplier", response.SupplierName);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldRejectSupplierThatIsNotAssignedToTheList()
    {
        var request = CreateItemQuoteTestHelper.CreateRequestDto();
        var shoppingItem = CreateItemQuoteTestHelper.CreateShoppingItemEntity();
        _helper.ShoppingItemRepositoryMock
            .Setup(x => x.GetByIdAsync(request.ShoppingItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shoppingItem);
        _helper.SetupShoppingListAccess(shoppingItem.ShoppingListId);
        _helper.ShoppingListSupplierRepositoryMock
            .Setup(x => x.ExistsAsync(
                shoppingItem.ShoppingListId,
                request.SupplierId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.ExecuteAsync(request));

        _helper.ItemQuoteRepositoryMock.Verify(
            repository => repository.AddAsync(
                It.IsAny<ItemQuoteEntity>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
