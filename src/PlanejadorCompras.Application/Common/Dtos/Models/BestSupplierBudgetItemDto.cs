namespace PlanejadorCompras.Application.Common.Dtos.Models;

public record BestSupplierBudgetItemDto(
    Guid ShoppingItemId,
    string Name,
    decimal UnitPrice,
    decimal Quantity,
    decimal TotalItemPrice
);
