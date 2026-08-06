using PlanejadorCompras.Application.Features.Reports.Contracts;
using PlanejadorCompras.Application.Features.QuotationRequests.Contracts;
using QuotationRequestEntity = PlanejadorCompras.Domain.Entities.QuotationRequest;

namespace PlanejadorCompras.Application.UseCases.QuotationRequest;

internal static class QuotationRequestResponseMapper
{
    public static QuotationRequestSummaryResponseDto ToSummary(
        QuotationRequestEntity request) =>
        new(
            request.Id,
            request.Code,
            request.SourceShoppingListId,
            request.ShoppingListName,
            request.BuyerName,
            request.Items.Count,
            request.ResponseDeadline,
            request.CreatedAtUtc);

    public static QuotationRequestDetailResponseDto ToDetail(
        QuotationRequestEntity request) =>
        new(
            request.Id,
            request.Code,
            request.SourceShoppingListId,
            request.ShoppingListName,
            request.Description,
            request.BuyerName,
            request.BuyerEmail,
            request.ResponseDeadline,
            request.DeliveryAddress,
            request.Instructions,
            request.CreatedAtUtc,
            request.Items
                .OrderBy(item => item.Position)
                .Select(item => new QuotationRequestItemResponseDto(
                    item.SourceShoppingItemId,
                    item.Name,
                    item.Quantity,
                    item.Unit))
                .ToList());

    public static QuotationRequestReportDataDto ToReport(
        QuotationRequestEntity request) =>
        new(
            request.SourceShoppingListId ?? Guid.Empty,
            request.Code,
            request.ShoppingListName,
            request.Description,
            request.BuyerName,
            request.BuyerEmail,
            DateOnly.FromDateTime(request.CreatedAtUtc),
            request.ResponseDeadline,
            request.DeliveryAddress,
            request.Instructions,
            request.Items
                .OrderBy(item => item.Position)
                .Select(item => new QuotationRequestReportItemDto(
                    item.Name,
                    item.Quantity,
                    item.Unit))
                .ToList());
}
