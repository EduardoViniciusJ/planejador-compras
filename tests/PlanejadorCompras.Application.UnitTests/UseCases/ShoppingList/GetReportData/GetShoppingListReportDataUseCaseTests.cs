using Moq;
using PlanejadorCompras.Application.Exceptions;
using Xunit;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ShoppingList.GetReportData;

public sealed class GetShoppingListReportDataUseCaseTests
{
    private static readonly DateTimeOffset GeneratedAt =
        new(2026, 7, 25, 18, 30, 0, TimeSpan.Zero);

    [Fact]
    public async Task ExecuteAsync_ShouldReturnConsolidatedReportData_WhenQuotesExist()
    {
        var helper = new GetShoppingListReportDataTestHelper(GeneratedAt);
        var useCase = helper.CreateUseCase();
        var listId = Guid.NewGuid();
        var paper = GetShoppingListReportDataTestHelper.CreateItem(listId, "Paper", 2m);
        var pen = GetShoppingListReportDataTestHelper.CreateItem(listId, "Pen", 1m);
        var quotes = new[]
        {
            GetShoppingListReportDataTestHelper.CreateQuote(
                paper,
                GetShoppingListReportDataTestHelper.SupplierA,
                10m),
            GetShoppingListReportDataTestHelper.CreateQuote(
                paper,
                GetShoppingListReportDataTestHelper.SupplierB,
                8m),
            GetShoppingListReportDataTestHelper.CreateQuote(
                pen,
                GetShoppingListReportDataTestHelper.SupplierA,
                5m),
            GetShoppingListReportDataTestHelper.CreateQuote(
                pen,
                GetShoppingListReportDataTestHelper.SupplierB,
                8m)
        };
        helper.SetupData(listId, new[] { paper, pen }, quotes);

        var result = await useCase.ExecuteAsync(listId);

        Assert.Equal(listId, result.ShoppingListId);
        Assert.Equal("Office supplies", result.Name);
        Assert.Equal("Monthly purchase", result.Description);
        Assert.Equal(GeneratedAt, result.GeneratedAt);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(2, result.Suppliers.Count);
        Assert.Empty(result.PendingItems);

        Assert.Equal(4, result.Summary.QuotedPriceCount);
        Assert.Equal(4, result.Summary.ExpectedPriceCount);
        Assert.Equal(100m, result.Summary.CoveragePercentage);
        Assert.Equal(21m, result.Summary.BestChoiceTotal);
        Assert.True(result.Summary.HasCompleteBestChoice);
        Assert.Equal(
            GetShoppingListReportDataTestHelper.SupplierB.Id,
            result.Summary.BestCompleteSupplierId);
        Assert.Equal("Supplier B", result.Summary.BestCompleteSupplierName);
        Assert.Equal(24m, result.Summary.BestCompleteSupplierTotal);
        Assert.Equal(3m, result.Summary.PotentialSavings);

        var paperRow = Assert.Single(result.Items, item => item.ShoppingItemId == paper.Id);
        Assert.Equal(8m, paperRow.LowestUnitPrice);
        Assert.Equal(16m, paperRow.LowestTotalPrice);
        Assert.Single(paperRow.Quotes, quote => quote.IsLowestPrice);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldUseLowestQuotesAndExposePendingPrices()
    {
        var helper = new GetShoppingListReportDataTestHelper(GeneratedAt);
        var useCase = helper.CreateUseCase();
        var listId = Guid.NewGuid();
        var paper = GetShoppingListReportDataTestHelper.CreateItem(listId, "Paper", 2m);
        var pen = GetShoppingListReportDataTestHelper.CreateItem(listId, "Pen", 1m);
        var quotes = new[]
        {
            GetShoppingListReportDataTestHelper.CreateQuote(
                paper,
                GetShoppingListReportDataTestHelper.SupplierA,
                10m),
            GetShoppingListReportDataTestHelper.CreateQuote(
                paper,
                GetShoppingListReportDataTestHelper.SupplierA,
                8m),
            GetShoppingListReportDataTestHelper.CreateQuote(
                pen,
                GetShoppingListReportDataTestHelper.SupplierB,
                5m)
        };
        helper.SetupData(listId, new[] { paper, pen }, quotes);

        var result = await useCase.ExecuteAsync(listId);

        Assert.Equal(2, result.Summary.QuotedPriceCount);
        Assert.Equal(4, result.Summary.ExpectedPriceCount);
        Assert.Equal(50m, result.Summary.CoveragePercentage);
        Assert.Equal(21m, result.Summary.BestChoiceTotal);
        Assert.True(result.Summary.HasCompleteBestChoice);
        Assert.Null(result.Summary.BestCompleteSupplierName);
        Assert.Null(result.Summary.BestCompleteSupplierTotal);
        Assert.Null(result.Summary.PotentialSavings);

        var paperRow = Assert.Single(result.Items, item => item.ShoppingItemId == paper.Id);
        Assert.Equal(8m, Assert.Single(paperRow.Quotes).UnitPrice);

        var paperPending = Assert.Single(
            result.PendingItems,
            item => item.ShoppingItemId == paper.Id);
        Assert.Equal(
            GetShoppingListReportDataTestHelper.SupplierB.Id,
            Assert.Single(paperPending.MissingSupplierIds));

        var penPending = Assert.Single(
            result.PendingItems,
            item => item.ShoppingItemId == pen.Id);
        Assert.Equal("Supplier A", Assert.Single(penPending.MissingSupplierNames));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnEmptyQuoteData_WhenNoQuotesExist()
    {
        var helper = new GetShoppingListReportDataTestHelper(GeneratedAt);
        var useCase = helper.CreateUseCase();
        var listId = Guid.NewGuid();
        var paper = GetShoppingListReportDataTestHelper.CreateItem(listId, "Paper", 2m);
        helper.SetupData(
            listId,
            new[] { paper },
            Array.Empty<PlanejadorCompras.Domain.Entities.ItemQuote>(),
            new[] { GetShoppingListReportDataTestHelper.SupplierA });

        var result = await useCase.ExecuteAsync(listId);

        Assert.Single(result.Suppliers);
        Assert.Single(result.Items);
        Assert.Empty(Assert.Single(result.Items).Quotes);
        var pendingItem = Assert.Single(result.PendingItems);
        Assert.Equal(
            "Supplier A",
            Assert.Single(pendingItem.MissingSupplierNames));
        Assert.Equal(0m, result.Summary.CoveragePercentage);
        Assert.Equal(1, result.Summary.ExpectedPriceCount);
        Assert.Equal(0m, result.Summary.BestChoiceTotal);
        Assert.False(result.Summary.HasCompleteBestChoice);
        Assert.Null(result.Summary.BestCompleteSupplierName);
        Assert.Null(result.Summary.PotentialSavings);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldPropagateNotFound_WhenListIsNotAccessible()
    {
        var helper = new GetShoppingListReportDataTestHelper(GeneratedAt);
        var useCase = helper.CreateUseCase();
        var listId = Guid.NewGuid();
        helper.ShoppingListAccessServiceMock
            .Setup(service => service.GetForCurrentUserAsync(
                listId,
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Shopping list not found."));

        await Assert.ThrowsAsync<NotFoundException>(() => useCase.ExecuteAsync(listId));

        helper.ShoppingItemRepositoryMock.Verify(
            repository => repository.GetByShoppingListIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
        helper.ItemQuoteRepositoryMock.Verify(
            repository => repository.GetByShoppingListIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldRejectEmptyId()
    {
        var helper = new GetShoppingListReportDataTestHelper(GeneratedAt);
        var useCase = helper.CreateUseCase();

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => useCase.ExecuteAsync(Guid.Empty));
    }
}
