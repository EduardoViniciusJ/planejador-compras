using PlanejadorCompras.Domain.Entities;

namespace PlanejadorCompras.Application.UnitTests.Domain;

public sealed class SavedEqualizationTests
{
    [Fact]
    public void Create_ShouldPreserveTheCompleteComparisonSnapshot()
    {
        var listId = Guid.NewGuid();
        var supplierA = Guid.NewGuid();
        var supplierB = Guid.NewGuid();
        var createdAtUtc = new DateTime(
            2026,
            7,
            30,
            15,
            0,
            0,
            DateTimeKind.Utc);

        var equalization = SavedEqualization.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            listId,
            "EQ-2026-ABC12345",
            "Compras para escritorio",
            "Marina Lopes",
            "marina@example.com",
            24m,
            "Fornecedor B",
            24m,
            0m,
            [
                new SavedEqualizationItemSnapshot(
                    Guid.NewGuid(),
                    0,
                    "Mouse",
                    2,
                    "un",
                    [
                        new SavedEqualizationQuoteSnapshot(
                            supplierA,
                            "Fornecedor A",
                            15m,
                            false),
                        new SavedEqualizationQuoteSnapshot(
                            supplierB,
                            "Fornecedor B",
                            12m,
                            true)
                    ])
            ],
            createdAtUtc);

        Assert.Equal(listId, equalization.SourceShoppingListId);
        Assert.Equal("EQ-2026-ABC12345", equalization.Code);
        Assert.Equal(createdAtUtc, equalization.CreatedAtUtc);
        Assert.Equal(2, equalization.SupplierCount);
        Assert.Single(equalization.Items);
        Assert.Equal(2, equalization.Items.Single().Quotes.Count);
        Assert.Equal(
            12m,
            equalization.Items.Single().Quotes.Single(quote => quote.IsLowest).UnitPrice);
    }

    [Fact]
    public void Create_ShouldRejectAnEmptySnapshot()
    {
        Assert.Throws<ArgumentException>(() =>
            SavedEqualization.Create(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                "EQ-2026-ABC12345",
                "Lista",
                "Marina",
                "marina@example.com",
                0m,
                null,
                null,
                0m,
                [],
                DateTime.UtcNow));
    }
}
