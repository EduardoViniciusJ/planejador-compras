using Moq;
using PlanejadorCompras.Application.UseCases.ShoppingList;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ShoppingList.GetByUserId;

public sealed class GetShoppingListsByUserIdUseCaseTests
{
    private readonly GetShoppingListsByUserIdTestHelper _helper;
    private readonly GetShoppingListsByUserIdUseCase _handler;

    public GetShoppingListsByUserIdUseCaseTests()
    {
        _helper = new GetShoppingListsByUserIdTestHelper();
        _handler = new GetShoppingListsByUserIdUseCase(
            _helper.ShoppingListOverviewQueryMock.Object,
            _helper.CurrentUserMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnShoppingLists_WhenUserHasLists()
    {
        var userId = GetShoppingListsByUserIdTestHelper.DefaultUserId;
        var shoppingLists = new List<PlanejadorCompras.Application.Common.Dtos.Models.ShoppingListOverviewDto>
        {
            GetShoppingListsByUserIdTestHelper.CreateOverview("Monthly Shopping List"),
            GetShoppingListsByUserIdTestHelper.CreateOverview("Office Setup List", 2, 1, 120m),
            GetShoppingListsByUserIdTestHelper.CreateOverview("Ready List", 3, 3, 350m)
        };

        _helper.ShoppingListOverviewQueryMock
            .Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shoppingLists);

        var response = await _handler.ExecuteAsync();

        Assert.Equal(3, response.Lists.Count);
        Assert.Equal(shoppingLists[0].Id, response.Lists[0].Id);
        Assert.Equal(3, response.Summary.TotalLists);
        Assert.Equal(1, response.Summary.DraftLists);
        Assert.Equal(1, response.Summary.AwaitingQuotesLists);
        Assert.Equal(1, response.Summary.ReadyForEqualizationLists);
        Assert.Equal(470m, response.Summary.TotalEstimated);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnEmptyList_WhenUserHasNoLists()
    {
        var userId = GetShoppingListsByUserIdTestHelper.DefaultUserId;
        _helper.ShoppingListOverviewQueryMock
            .Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<PlanejadorCompras.Application.Common.Dtos.Models.ShoppingListOverviewDto>());

        var response = await _handler.ExecuteAsync();

        Assert.Empty(response.Lists);
        Assert.Equal(0, response.Summary.TotalLists);
        Assert.Equal(0m, response.Summary.TotalEstimated);
    }


    [Fact]
    public async Task ExecuteAsync_ShouldCallRepositoryWithCorrectUserId()
    {
        var userId = GetShoppingListsByUserIdTestHelper.DefaultUserId;
        _helper.ShoppingListOverviewQueryMock
            .Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<PlanejadorCompras.Application.Common.Dtos.Models.ShoppingListOverviewDto>());

        await _handler.ExecuteAsync();

        _helper.ShoppingListOverviewQueryMock.Verify(
            x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
