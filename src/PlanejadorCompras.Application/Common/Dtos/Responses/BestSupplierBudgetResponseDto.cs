namespace PlanejadorCompras.Application.Common.Dtos.Responses;

public record BestSupplierBudgetResponseDto(
    Guid ShoppingListId,
    string? BestSupplierName,
    decimal TotalPrice,
    IEnumerable<BestSupplierBudgetItemDto> Items
);
