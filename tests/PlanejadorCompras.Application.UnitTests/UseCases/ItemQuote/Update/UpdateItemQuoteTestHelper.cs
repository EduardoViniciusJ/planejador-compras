using Moq;
using PlanejadorCompras.Application.Common.Dtos.Requests;
using PlanejadorCompras.Domain.Repositories;
using PlanejadorCompras.Domain.Repositories.ItemQuote;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ItemQuote.Update;

public sealed class UpdateItemQuoteTestHelper
{
    public UpdateItemQuoteTestHelper()
    {
        ItemQuoteRepositoryMock = new Mock<IItemQuoteRepository>();
        UnitOfWorkMock = new Mock<IUnitOfWork>();
        UnitOfWorkMock
            .Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    public static Guid DefaultShoppingItemId => Guid.Parse("66666666-6666-6666-6666-666666666666");

    public Mock<IItemQuoteRepository> ItemQuoteRepositoryMock { get; }

    public Mock<IUnitOfWork> UnitOfWorkMock { get; }

    public static ItemQuoteRequestDto CreateRequestDto(
        Guid? shoppingItemId = null,
        string supplierName = "Updated Supplier",
        decimal unitPrice = 175.50m)
    {
        return new ItemQuoteRequestDto(shoppingItemId ?? DefaultShoppingItemId, supplierName, unitPrice);
    }
}
