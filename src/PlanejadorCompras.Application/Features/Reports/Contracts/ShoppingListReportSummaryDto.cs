namespace PlanejadorCompras.Application.Features.Reports.Contracts;

public sealed record ShoppingListReportSummaryDto(
    int TotalItems,
    int TotalSuppliers,
    int QuotedItems,
    int QuotedPriceCount,
    int ExpectedPriceCount,
    decimal CoveragePercentage,
    decimal BestChoiceTotal,
    bool HasCompleteBestChoice,
    Guid? BestCompleteSupplierId,
    string? BestCompleteSupplierName,
    decimal? BestCompleteSupplierTotal,
    decimal? PotentialSavings);
