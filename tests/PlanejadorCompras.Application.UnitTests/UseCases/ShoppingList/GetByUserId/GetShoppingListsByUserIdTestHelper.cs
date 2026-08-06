using PlanejadorCompras.Application.Features.ShoppingLists.Contracts;
using Moq;
using PlanejadorCompras.Application.Services.Interfaces;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ShoppingList.GetByUserId;

public sealed class GetShoppingListsByUserIdTestHelper
{
    public GetShoppingListsByUserIdTestHelper()
    {
        ShoppingListOverviewQueryMock = new Mock<IShoppingListOverviewQuery>();

        CurrentUserMock = new Mock<ICurrentUser>();
        CurrentUserMock
            .Setup(x => x.UserId)
            .Returns(DefaultUserId);
    }

    public static Guid DefaultUserId => Guid.Parse("11111111-1111-1111-1111-111111111111");

    public Mock<IShoppingListOverviewQuery> ShoppingListOverviewQueryMock { get; }

    public Mock<ICurrentUser> CurrentUserMock { get; }

    public static ShoppingListOverviewDto CreateOverview(
        string name = "Monthly Tech Shopping",
        int itemCount = 0,
        int quotedItemCount = 0,
        decimal estimatedTotal = 0m)
    {
        return new ShoppingListOverviewDto(
            Guid.NewGuid(),
            name,
            "Monitor, keyboard, and mouse",
            DateTime.UtcNow,
            itemCount,
            quotedItemCount,
            estimatedTotal);
    }
}
