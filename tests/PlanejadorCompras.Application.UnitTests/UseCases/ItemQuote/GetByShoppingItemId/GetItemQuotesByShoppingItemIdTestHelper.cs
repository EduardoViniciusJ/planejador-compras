using Moq;
using PlanejadorCompras.Domain.Repositories.ItemQuote;
using ItemQuoteEntity = PlanejadorCompras.Domain.Entities.ItemQuote;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ItemQuote.GetByShoppingItemId;

public sealed class GetItemQuotesByShoppingItemIdTestHelper
{
    public GetItemQuotesByShoppingItemIdTestHelper()
    {
        ItemQuoteRepositoryMock = new Mock<IItemQuoteRepository>();
    }

    public static Guid DefaultShoppingItemId => Guid.Parse("66666666-6666-6666-6666-666666666666");

    public Mock<IItemQuoteRepository> ItemQuoteRepositoryMock { get; }

    public static ItemQuoteEntity CreateItemQuoteEntity(
        Guid? shoppingItemId = null,
        string supplierName = "Best Monitor Supplier",
        decimal unitPrice = 199.90m)
    {
        return ItemQuoteEntity.Create(shoppingItemId ?? DefaultShoppingItemId, supplierName, unitPrice);
    }
}
