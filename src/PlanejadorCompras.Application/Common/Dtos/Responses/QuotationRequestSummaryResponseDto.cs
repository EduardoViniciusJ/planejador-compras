namespace PlanejadorCompras.Application.Common.Dtos.Responses;

public sealed record QuotationRequestSummaryResponseDto(
    Guid Id,
    string Code,
    Guid? SourceShoppingListId,
    string ShoppingListName,
    string BuyerName,
    int ItemCount,
    DateOnly? ResponseDeadline,
    DateTime CreatedAtUtc);
