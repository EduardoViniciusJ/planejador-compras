namespace PlanejadorCompras.Application.Features.Reports.Contracts;

public sealed record QuotationRequestReportDataDto(
    Guid ShoppingListId,
    string Code,
    string ShoppingListName,
    string? Description,
    string BuyerName,
    string BuyerEmail,
    DateOnly IssuedOn,
    DateOnly? ResponseDeadline,
    string? DeliveryAddress,
    string? Instructions,
    IReadOnlyCollection<QuotationRequestReportItemDto> Items);

public sealed record QuotationRequestReportItemDto(
    string Name,
    decimal Quantity,
    string Unit);
