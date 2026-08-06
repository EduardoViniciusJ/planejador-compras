namespace PlanejadorCompras.Application.Features.QuotationRequests.Contracts;

public sealed record QuotationRequestSummaryResponseDto(
    Guid Id,
    string Code,
    Guid? SourceShoppingListId,
    string ShoppingListName,
    string BuyerName,
    int ItemCount,
    DateOnly? ResponseDeadline,
    DateTime CreatedAtUtc);
