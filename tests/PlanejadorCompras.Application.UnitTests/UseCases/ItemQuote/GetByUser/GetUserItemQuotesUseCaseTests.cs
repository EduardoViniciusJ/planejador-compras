using Moq;
using PlanejadorCompras.Application.Common.Dtos.Models;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Application.UseCases.ItemQuote;
using Xunit;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ItemQuote.GetByUser;

public sealed class GetUserItemQuotesUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldReturnOnlyQueryResultsForCurrentUser()
    {
        var userId = Guid.NewGuid();
        var currentUser = new Mock<ICurrentUser>();
        currentUser.Setup(x => x.UserId).Returns(userId);
        var query = new Mock<IUserItemQuotesQuery>();
        var quote = new UserItemQuoteDto(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Office",
            Guid.NewGuid(),
            "Paper",
            2m,
            "box",
            Guid.NewGuid(),
            "Supplier",
            10m,
            DateTime.UtcNow);
        query.Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserItemQuoteDto> { quote });

        var useCase = new GetUserItemQuotesUseCase(currentUser.Object, query.Object);
        var result = await useCase.ExecuteAsync();

        Assert.Same(quote, Assert.Single(result.Quotes));
        query.Verify(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
