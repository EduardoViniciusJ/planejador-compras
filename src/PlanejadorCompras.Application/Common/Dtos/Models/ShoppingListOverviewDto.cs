namespace PlanejadorCompras.Application.Common.Dtos.Models;

public sealed record ShoppingListOverviewDto(
    Guid Id,
    string Name,
    string? Description,
    DateTime CreatedAt,
    int ItemCount,
    int QuotedItemCount,
    decimal EstimatedTotal);
