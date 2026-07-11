namespace PlanejadorCompras.Application.Common.Dtos.Models;

public sealed record ShoppingListsSummaryDto(
    int TotalLists,
    int DraftLists,
    int AwaitingQuotesLists,
    int ReadyForEqualizationLists,
    decimal TotalEstimated);
