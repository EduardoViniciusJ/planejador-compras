using Moq;
using PlanejadorCompras.Application.Common.Dtos.Requests;
using PlanejadorCompras.Application.UseCases.ShoppingList.Create;
using ShoppingListEntity = PlanejadorCompras.Domain.Entities.ShoppingList;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ShoppingList.Create;

public sealed class CreateShoppingListUseCaseTests
{
    private readonly CreateShoppingListTestHelper _helper;
    private readonly CreateShoppingListUseCase _handler;

    public CreateShoppingListUseCaseTests()
    {
        _helper = new CreateShoppingListTestHelper();
        _handler = new CreateShoppingListUseCase(
            _helper.ShoppingListRepositoryMock.Object,
            _helper.UnitOfWorkMock.Object,
            _helper.CurrentUserMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCreateShoppingList_WhenRequestIsValid()
    {
        var request = new ShoppingListRequestDto("Monthly Tech Shopping", "Monitor, keyboard, and mouse");

        var response = await _handler.ExecuteAsync(request);

        Assert.Equal(request.Name, response.Name);
        Assert.Equal(request.Description, response.Description);
        Assert.NotEqual(Guid.Empty, response.Id);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCallRepositoryWithCorrectData()
    {
        var request = CreateShoppingListTestHelper.CreateRequestDto("Monthly Shopping List", "Monitor and printer ink");

        await _handler.ExecuteAsync(request);

        _helper.ShoppingListRepositoryMock.Verify(
            x => x.AddAsync(
                It.Is<ShoppingListEntity>(s =>
                    s.UserId == CreateShoppingListTestHelper.DefaultUserId &&
                    s.Name == request.Name &&
                    s.Description == request.Description),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.ExecuteAsync(null!));
    }


    [Fact]
    public async Task ExecuteAsync_ShouldTrimNameAndDescription()
    {
        var request = new ShoppingListRequestDto("  Office Monitor List  ", "  Monitor stand and HDMI cable  ");

        var response = await _handler.ExecuteAsync(request);

        Assert.Equal("Office Monitor List", response.Name);
        Assert.Equal("Monitor stand and HDMI cable", response.Description);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldSetCreatedAtToCurrentUtcTime()
    {
        var request = CreateShoppingListTestHelper.CreateRequestDto();
        var beforeExecution = DateTime.UtcNow.AddSeconds(-1);

        var response = await _handler.ExecuteAsync(request);

        var afterExecution = DateTime.UtcNow.AddSeconds(1);
        Assert.True(response.CreatedAt >= beforeExecution && response.CreatedAt <= afterExecution);
    }
}
