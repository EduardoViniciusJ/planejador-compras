namespace PlanejadorCompras.Application.Features.ShoppingLists.Contracts;

public sealed record ShoppingListOverviewDto(
    Guid Id,
    string Name,
    string? Description,
    DateTime CreatedAt,
    int ItemCount,
    int QuotedItemCount,
    decimal EstimatedTotal);
