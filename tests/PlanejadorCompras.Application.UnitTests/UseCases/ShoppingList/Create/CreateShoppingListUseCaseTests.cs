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
            _helper.UnitOfWorkMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCreateShoppingList_WhenRequestIsValid()
    {
        var request = new ShoppingListRequestDto("Monthly Tech Shopping", "Monitor, keyboard, and mouse");
        var userId = Guid.NewGuid();

        var response = await _handler.ExecuteAsync(request, userId);

        Assert.Equal(userId, response.UserId);
        Assert.Equal(request.Name, response.Name);
        Assert.Equal(request.Description, response.Description);
        Assert.NotEqual(Guid.Empty, response.Id);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCallRepositoryWithCorrectData()
    {
        var request = CreateShoppingListTestHelper.CreateRequestDto("Monthly Shopping List", "Monitor and printer ink");
        var userId = Guid.NewGuid();

        await _handler.ExecuteAsync(request, userId);

        _helper.ShoppingListRepositoryMock.Verify(
            x => x.AddAsync(
                It.Is<ShoppingListEntity>(s =>
                    s.UserId == userId &&
                    s.Name == request.Name &&
                    s.Description == request.Description),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.ExecuteAsync(null!, Guid.NewGuid()));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowArgumentOutOfRangeException_WhenUserIdIsEmpty()
    {
        var request = CreateShoppingListTestHelper.CreateRequestDto();

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _handler.ExecuteAsync(request, Guid.Empty));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldTrimNameAndDescription()
    {
        var request = new ShoppingListRequestDto("  Office Monitor List  ", "  Monitor stand and HDMI cable  ");
        var userId = CreateShoppingListTestHelper.DefaultUserId;

        var response = await _handler.ExecuteAsync(request, userId);

        Assert.Equal("Office Monitor List", response.Name);
        Assert.Equal("Monitor stand and HDMI cable", response.Description);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldSetCreatedAtToCurrentUtcTime()
    {
        var request = CreateShoppingListTestHelper.CreateRequestDto();
        var userId = CreateShoppingListTestHelper.DefaultUserId;
        var beforeExecution = DateTime.UtcNow.AddSeconds(-1);

        var response = await _handler.ExecuteAsync(request, userId);

        var afterExecution = DateTime.UtcNow.AddSeconds(1);
        Assert.True(response.CreatedAt >= beforeExecution && response.CreatedAt <= afterExecution);
    }
}
