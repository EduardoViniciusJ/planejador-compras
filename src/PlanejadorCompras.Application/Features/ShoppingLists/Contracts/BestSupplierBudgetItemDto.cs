namespace PlanejadorCompras.Application.Features.ShoppingLists.Contracts;

public record BestSupplierBudgetItemDto(
    Guid ShoppingItemId,
    string Name,
    decimal UnitPrice,
    decimal Quantity,
    decimal TotalItemPrice
);
