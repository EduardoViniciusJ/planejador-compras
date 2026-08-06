using PlanejadorCompras.Domain.Entities;
using PlanejadorCompras.Domain.Rules;

namespace PlanejadorCompras.Application.UnitTests.Domain.Validation;

public sealed class CommercialValueValidationTests
{
    [Fact]
    public void ShoppingItem_ShouldAcceptConfiguredQuantityBoundaries()
    {
        var listId = Guid.NewGuid();

        var minimum = ShoppingItem.Create(
            listId,
            "Item mínimo",
            ShoppingItemRules.MinimumQuantity,
            "un");
        var maximum = ShoppingItem.Create(
            listId,
            "Item máximo",
            ShoppingItemRules.MaximumQuantity,
            "un");

        Assert.Equal(ShoppingItemRules.MinimumQuantity, minimum.Quantity);
        Assert.Equal(ShoppingItemRules.MaximumQuantity, maximum.Quantity);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("10000000000000000")]
    public void ShoppingItem_ShouldRejectQuantityOutsideDatabaseRange(string value)
    {
        var quantity = decimal.Parse(value, System.Globalization.CultureInfo.InvariantCulture);

        Assert.Throws<ArgumentOutOfRangeException>(() => ShoppingItem.Create(
            Guid.NewGuid(),
            "Item",
            quantity,
            "un"));
    }

    [Fact]
    public void ItemQuote_ShouldAcceptConfiguredPriceBoundaries()
    {
        var minimum = ItemQuote.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            ItemQuoteRules.MinimumUnitPrice);
        var maximum = ItemQuote.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            ItemQuoteRules.MaximumUnitPrice);

        Assert.Equal(ItemQuoteRules.MinimumUnitPrice, minimum.UnitPrice);
        Assert.Equal(ItemQuoteRules.MaximumUnitPrice, maximum.UnitPrice);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("10000000000000000")]
    public void ItemQuote_ShouldRejectPriceOutsideDatabaseRange(string value)
    {
        var price = decimal.Parse(value, System.Globalization.CultureInfo.InvariantCulture);

        Assert.Throws<ArgumentOutOfRangeException>(() => ItemQuote.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            price));
    }
}
