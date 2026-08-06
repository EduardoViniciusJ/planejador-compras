using PlanejadorCompras.Application.Features.ShoppingLists.Contracts;
using Moq;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Application.UseCases.ShoppingList;
using Xunit;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ShoppingList.GetDetail;

public sealed class GetShoppingListDetailUseCaseTests
{
    private static readonly Guid UserId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    [Fact]
    public async Task ExecuteAsync_ShouldReturnDetail_ForCurrentUser()
    {
        var listId = Guid.NewGuid();
        var currentUser = CreateCurrentUser();
        var query = new Mock<IShoppingListDetailQuery>();
        var expected = new ShoppingListDetailResponseDto(
            listId,
            "Office",
            null,
            DateTime.UtcNow,
            0,
            0,
            0m,
            []);
        query.Setup(x => x.GetByIdAsync(UserId, listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var useCase = new GetShoppingListDetailUseCase(currentUser.Object, query.Object);
        var result = await useCase.ExecuteAsync(listId);

        Assert.Same(expected, result);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowNotFound_WhenQueryDoesNotReturnList()
    {
        var listId = Guid.NewGuid();
        var currentUser = CreateCurrentUser();
        var query = new Mock<IShoppingListDetailQuery>();
        query.Setup(x => x.GetByIdAsync(UserId, listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ShoppingListDetailResponseDto?)null);

        var useCase = new GetShoppingListDetailUseCase(currentUser.Object, query.Object);

        await Assert.ThrowsAsync<NotFoundException>(() => useCase.ExecuteAsync(listId));
    }

    private static Mock<ICurrentUser> CreateCurrentUser()
    {
        var currentUser = new Mock<ICurrentUser>();
        currentUser.Setup(x => x.UserId).Returns(UserId);
        return currentUser;
    }
}
