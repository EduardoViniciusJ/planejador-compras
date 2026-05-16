namespace PlanejadorCompras.Application.Common.Dtos.Responses;

public record BestSupplierBudgetItemDto(
    Guid ShoppingItemId,
    string Name,
    decimal UnitPrice,
    decimal Quantity,
    decimal TotalItemPrice
);
