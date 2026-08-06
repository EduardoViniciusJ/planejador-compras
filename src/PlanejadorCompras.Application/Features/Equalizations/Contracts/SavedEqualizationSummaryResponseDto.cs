namespace PlanejadorCompras.Application.Features.Equalizations.Contracts;

public sealed record SavedEqualizationSummaryResponseDto(
    Guid Id,
    string Code,
    Guid ShoppingListId,
    string ShoppingListName,
    string CreatedByName,
    string CreatedByEmail,
    int ItemCount,
    int SupplierCount,
    decimal BestChoiceTotal,
    string? BestCompleteSupplierName,
    decimal? BestCompleteSupplierTotal,
    decimal EstimatedEconomy,
    DateTime CreatedAtUtc);
