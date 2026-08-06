namespace PlanejadorCompras.Application.Features.QuotationRequests.Contracts;

public sealed record QuotationRequestDetailResponseDto(
    Guid Id,
    string Code,
    Guid? SourceShoppingListId,
    string ShoppingListName,
    string? Description,
    string BuyerName,
    string BuyerEmail,
    DateOnly? ResponseDeadline,
    string? DeliveryAddress,
    string? Instructions,
    DateTime CreatedAtUtc,
    IReadOnlyCollection<QuotationRequestItemResponseDto> Items);

public sealed record QuotationRequestItemResponseDto(
    Guid? SourceShoppingItemId,
    string Name,
    decimal Quantity,
    string Unit);
