namespace PlanejadorCompras.Application.Features.Equalizations.Contracts;

public sealed record SavedEqualizationDetailResponseDto(
    Guid Id,
    string Code,
    Guid ShoppingListId,
    string ShoppingListName,
    string CreatedByName,
    string CreatedByEmail,
    decimal BestChoiceTotal,
    string? BestCompleteSupplierName,
    decimal? BestCompleteSupplierTotal,
    decimal EstimatedEconomy,
    DateTime CreatedAtUtc,
    IReadOnlyCollection<string> Suppliers,
    IReadOnlyCollection<SavedEqualizationItemResponseDto> Items);
