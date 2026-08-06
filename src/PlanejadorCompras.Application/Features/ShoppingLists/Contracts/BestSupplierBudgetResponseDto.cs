
namespace PlanejadorCompras.Application.Features.ShoppingLists.Contracts;

public record BestSupplierBudgetResponseDto(
    Guid ShoppingListId,
    string? BestSupplierName,
    decimal TotalPrice,
    IEnumerable<BestSupplierBudgetItemDto> Items
);
