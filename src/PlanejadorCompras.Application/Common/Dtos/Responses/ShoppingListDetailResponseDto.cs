using PlanejadorCompras.Application.Common.Dtos.Models;

namespace PlanejadorCompras.Application.Common.Dtos.Responses;

public sealed record ShoppingListDetailResponseDto(
    Guid Id,
    string Name,
    string? Description,
    DateTime CreatedAt,
    int TotalItems,
    int QuotedItems,
    decimal TotalEstimated,
    IReadOnlyCollection<ShoppingListDetailItemDto> Items);
