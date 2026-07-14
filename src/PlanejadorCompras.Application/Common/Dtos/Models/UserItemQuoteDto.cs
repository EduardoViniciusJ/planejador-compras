namespace PlanejadorCompras.Application.Common.Dtos.Models;

public sealed record UserItemQuoteDto(
    Guid Id,
    Guid ShoppingListId,
    string ShoppingListName,
    Guid ShoppingItemId,
    string ShoppingItemName,
    decimal Quantity,
    string Unit,
    Guid SupplierId,
    string SupplierName,
    decimal UnitPrice,
    DateTime CreatedAt);
