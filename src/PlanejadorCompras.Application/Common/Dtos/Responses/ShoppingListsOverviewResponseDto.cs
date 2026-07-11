using PlanejadorCompras.Application.Common.Dtos.Models;

namespace PlanejadorCompras.Application.Common.Dtos.Responses;

public sealed record ShoppingListsOverviewResponseDto(
    ShoppingListsSummaryDto Summary,
    IReadOnlyList<ShoppingListOverviewDto> Lists);
