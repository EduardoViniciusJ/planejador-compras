using PlanejadorCompras.Application.Features.ShoppingItems.Contracts;
using Moq;
using PlanejadorCompras.Application.UseCases.ShoppingItem.Create;
using ShoppingItemEntity = PlanejadorCompras.Domain.Entities.ShoppingItem;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ShoppingItem.Create;

public sealed class CreateShoppingItemUseCaseTests
{
    private readonly CreateShoppingItemTestHelper _helper;
    private readonly CreateShoppingItemUseCase _handler;

    public CreateShoppingItemUseCaseTests()
    {
        _helper = new CreateShoppingItemTestHelper();
        _handler = new CreateShoppingItemUseCase(
            _helper.ShoppingItemRepositoryMock.Object,
            _helper.UnitOfWorkMock.Object,
            _helper.ShoppingListAccessServiceMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCreateShoppingItem_WhenRequestIsValid()
    {
        var request = new ShoppingItemRequestDto(
            CreateShoppingItemTestHelper.DefaultShoppingListId,
            "Monthly Tech Shopping Item",
            2,
            "pcs");

        _helper.SetupShoppingListAccess(request.ShoppingListId);

        var response = await _handler.ExecuteAsync(request);

        Assert.Equal(request.ShoppingListId, response.ShoppingListId);
        Assert.Equal(request.Name, response.Name);
        Assert.Equal(request.Quantity, response.Quantity);
        Assert.Equal(request.Unit, response.Unit);
        Assert.NotEqual(Guid.Empty, response.Id);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCallRepositoryWithCorrectData()
    {
        var request = CreateShoppingItemTestHelper.CreateRequestDto();

        _helper.SetupShoppingListAccess(request.ShoppingListId);

        await _handler.ExecuteAsync(request);

        _helper.ShoppingItemRepositoryMock.Verify(
            x => x.AddAsync(
                It.Is<ShoppingItemEntity>(s =>
                    s.ShoppingListId == request.ShoppingListId &&
                    s.Name == request.Name &&
                    s.Quantity == request.Quantity &&
                    s.Unit == request.Unit),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCommitUnitOfWork_WhenCreationSucceeds()
    {
        var request = CreateShoppingItemTestHelper.CreateRequestDto();

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
    public async Task ExecuteAsync_ShouldTrimNameAndUnit()
    {
        var request = new ShoppingItemRequestDto(
            CreateShoppingItemTestHelper.DefaultShoppingListId,
            "  Monitor Arm  ",
            1,
            "  pcs  ");

        _helper.SetupShoppingListAccess(request.ShoppingListId);

        var response = await _handler.ExecuteAsync(request);

        Assert.Equal("Monitor Arm", response.Name);
        Assert.Equal("pcs", response.Unit);
    }
}
