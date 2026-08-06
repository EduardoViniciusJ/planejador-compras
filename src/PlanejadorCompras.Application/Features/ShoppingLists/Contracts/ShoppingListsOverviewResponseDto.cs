
namespace PlanejadorCompras.Application.Features.ShoppingLists.Contracts;

public sealed record ShoppingListsOverviewResponseDto(
    ShoppingListsSummaryDto Summary,
    IReadOnlyList<ShoppingListOverviewDto> Lists);
