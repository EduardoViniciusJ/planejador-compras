using Moq;
using PlanejadorCompras.Application.Exceptions;
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
            _helper.ShoppingItemRepositoryMock.Object,
            _helper.UnitOfWorkMock.Object,
            _helper.ShoppingListAccessServiceMock.Object,
            _helper.SupplierAccessServiceMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldUpdateItemQuote_WhenRequestIsValid()
    {
        var itemQuote = ItemQuoteEntity.Create(UpdateItemQuoteTestHelper.DefaultShoppingItemId, Guid.NewGuid(), 199.90m);
        var request = UpdateItemQuoteTestHelper.CreateRequestDto(itemQuote.ShoppingItemId, UpdateItemQuoteTestHelper.DefaultSupplier.Id, 175.50m);
        var currentShoppingItem = UpdateItemQuoteTestHelper.CreateShoppingItemEntity();
        var targetShoppingItem = UpdateItemQuoteTestHelper.CreateShoppingItemEntity(itemQuote.ShoppingItemId);

        _helper.ItemQuoteRepositoryMock
            .Setup(x => x.GetByIdAsync(itemQuote.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(itemQuote);
        _helper.ShoppingItemRepositoryMock
            .Setup(x => x.GetByIdAsync(itemQuote.ShoppingItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currentShoppingItem);
        _helper.ShoppingItemRepositoryMock
            .Setup(x => x.GetByIdAsync(request.ShoppingItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(targetShoppingItem);
        _helper.ShoppingListAccessServiceMock
            .Setup(x => x.GetForCurrentUserAsync(currentShoppingItem.ShoppingListId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(PlanejadorCompras.Domain.Entities.ShoppingList.Create(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Authorized List"));
        _helper.ShoppingListAccessServiceMock
            .Setup(x => x.GetForCurrentUserAsync(targetShoppingItem.ShoppingListId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(PlanejadorCompras.Domain.Entities.ShoppingList.Create(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Authorized List"));

        var response = await _handler.ExecuteAsync(itemQuote.Id, request);

        Assert.NotNull(response);
        Assert.Equal(itemQuote.Id, response.Id);
        Assert.Equal(request.ShoppingItemId, response.ShoppingItemId);
        Assert.Equal(request.SupplierId, response.SupplierId);
        Assert.Equal(UpdateItemQuoteTestHelper.DefaultSupplier.Name, response.SupplierName);
        Assert.Equal(request.UnitPrice, response.UnitPrice);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCallUpdateRepositoryWithCorrectData()
    {
        var itemQuote = ItemQuoteEntity.Create(UpdateItemQuoteTestHelper.DefaultShoppingItemId, Guid.NewGuid(), 199.90m);
        var request = UpdateItemQuoteTestHelper.CreateRequestDto(itemQuote.ShoppingItemId, UpdateItemQuoteTestHelper.DefaultSupplier.Id, 175.50m);
        var currentShoppingItem = UpdateItemQuoteTestHelper.CreateShoppingItemEntity();
        var targetShoppingItem = UpdateItemQuoteTestHelper.CreateShoppingItemEntity(itemQuote.ShoppingItemId);

        _helper.ItemQuoteRepositoryMock
            .Setup(x => x.GetByIdAsync(itemQuote.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(itemQuote);
        _helper.ShoppingItemRepositoryMock
            .Setup(x => x.GetByIdAsync(itemQuote.ShoppingItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currentShoppingItem);
        _helper.ShoppingItemRepositoryMock
            .Setup(x => x.GetByIdAsync(request.ShoppingItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(targetShoppingItem);
        _helper.ShoppingListAccessServiceMock
            .Setup(x => x.GetForCurrentUserAsync(currentShoppingItem.ShoppingListId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(PlanejadorCompras.Domain.Entities.ShoppingList.Create(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Authorized List"));
        _helper.ShoppingListAccessServiceMock
            .Setup(x => x.GetForCurrentUserAsync(targetShoppingItem.ShoppingListId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(PlanejadorCompras.Domain.Entities.ShoppingList.Create(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Authorized List"));

        await _handler.ExecuteAsync(itemQuote.Id, request);

        _helper.ItemQuoteRepositoryMock.Verify(
            x => x.UpdateAsync(
                It.Is<ItemQuoteEntity>(iq =>
                    iq.Id == itemQuote.Id &&
                    iq.ShoppingItemId == request.ShoppingItemId &&
                    iq.SupplierId == request.SupplierId &&
                    iq.UnitPrice == request.UnitPrice),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCommitUnitOfWork_WhenUpdateSucceeds()
    {
        var itemQuote = ItemQuoteEntity.Create(UpdateItemQuoteTestHelper.DefaultShoppingItemId, Guid.NewGuid(), 199.90m);
        var request = UpdateItemQuoteTestHelper.CreateRequestDto(itemQuote.ShoppingItemId);
        var currentShoppingItem = UpdateItemQuoteTestHelper.CreateShoppingItemEntity();
        var targetShoppingItem = UpdateItemQuoteTestHelper.CreateShoppingItemEntity(itemQuote.ShoppingItemId);

        _helper.ItemQuoteRepositoryMock
            .Setup(x => x.GetByIdAsync(itemQuote.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(itemQuote);
        _helper.ShoppingItemRepositoryMock
            .Setup(x => x.GetByIdAsync(itemQuote.ShoppingItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currentShoppingItem);
        _helper.ShoppingItemRepositoryMock
            .Setup(x => x.GetByIdAsync(request.ShoppingItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(targetShoppingItem);
        _helper.ShoppingListAccessServiceMock
            .Setup(x => x.GetForCurrentUserAsync(currentShoppingItem.ShoppingListId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(PlanejadorCompras.Domain.Entities.ShoppingList.Create(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Authorized List"));
        _helper.ShoppingListAccessServiceMock
            .Setup(x => x.GetForCurrentUserAsync(targetShoppingItem.ShoppingListId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(PlanejadorCompras.Domain.Entities.ShoppingList.Create(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Authorized List"));

        await _handler.ExecuteAsync(itemQuote.Id, request);

        _helper.UnitOfWorkMock.Verify(
            x => x.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowNotFoundException_WhenItemQuoteDoesNotExist()
    {
        var itemQuoteId = Guid.NewGuid();
        var request = UpdateItemQuoteTestHelper.CreateRequestDto();

        _helper.ItemQuoteRepositoryMock
            .Setup(x => x.GetByIdAsync(itemQuoteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ItemQuoteEntity?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.ExecuteAsync(itemQuoteId, request));
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

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.ExecuteAsync(itemQuoteId, request));

        _helper.ItemQuoteRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<ItemQuoteEntity>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _helper.UnitOfWorkMock.Verify(
            x => x.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowNotFoundException_WhenTargetShoppingItemDoesNotBelongToUser()
    {
        var itemQuote = ItemQuoteEntity.Create(UpdateItemQuoteTestHelper.DefaultShoppingItemId, Guid.NewGuid(), 199.90m);
        var unauthorizedListId = Guid.NewGuid();
        var request = UpdateItemQuoteTestHelper.CreateRequestDto(unauthorizedListId);
        var currentShoppingItem = UpdateItemQuoteTestHelper.CreateShoppingItemEntity();

        _helper.ItemQuoteRepositoryMock
            .Setup(x => x.GetByIdAsync(itemQuote.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(itemQuote);
        _helper.ShoppingItemRepositoryMock
            .Setup(x => x.GetByIdAsync(itemQuote.ShoppingItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currentShoppingItem);
        _helper.ShoppingItemRepositoryMock
            .Setup(x => x.GetByIdAsync(request.ShoppingItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(UpdateItemQuoteTestHelper.CreateShoppingItemEntity(unauthorizedListId));
        _helper.ShoppingListAccessServiceMock
            .Setup(x => x.GetForCurrentUserAsync(currentShoppingItem.ShoppingListId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(PlanejadorCompras.Domain.Entities.ShoppingList.Create(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Authorized List"));
        _helper.ShoppingListAccessServiceMock
            .Setup(x => x.GetForCurrentUserAsync(unauthorizedListId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Shopping list not found.", "shopping_list_not_found"));

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.ExecuteAsync(itemQuote.Id, request));
    }
}
