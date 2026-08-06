namespace PlanejadorCompras.Application.Features.ShoppingLists.Contracts;

public sealed record ShoppingListsSummaryDto(
    int TotalLists,
    int DraftLists,
    int AwaitingQuotesLists,
    int ReadyForEqualizationLists,
    decimal TotalEstimated);
