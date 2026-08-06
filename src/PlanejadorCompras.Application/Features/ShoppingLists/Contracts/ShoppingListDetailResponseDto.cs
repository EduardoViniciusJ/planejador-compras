
namespace PlanejadorCompras.Application.Features.ShoppingLists.Contracts;

public sealed record ShoppingListDetailResponseDto(
    Guid Id,
    string Name,
    string? Description,
    DateTime CreatedAt,
    int TotalItems,
    int QuotedItems,
    decimal TotalEstimated,
    IReadOnlyCollection<ShoppingListDetailItemDto> Items);
