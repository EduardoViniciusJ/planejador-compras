using PlanejadorCompras.Application.Features.Equalizations.Contracts;
using PlanejadorCompras.Domain.Entities;

namespace PlanejadorCompras.Application.UseCases.Equalization;

internal static class SavedEqualizationResponseMapper
{
    public static SavedEqualizationSummaryResponseDto ToSummary(
        SavedEqualization equalization) =>
        new(
            equalization.Id,
            equalization.Code,
            equalization.SourceShoppingListId,
            equalization.ShoppingListName,
            equalization.CreatedByName,
            equalization.CreatedByEmail,
            equalization.Items.Count,
            equalization.SupplierCount,
            equalization.BestChoiceTotal,
            equalization.BestCompleteSupplierName,
            equalization.BestCompleteSupplierTotal,
            equalization.EstimatedEconomy,
            equalization.CreatedAtUtc);

    public static SavedEqualizationDetailResponseDto ToDetail(
        SavedEqualization equalization)
    {
        var suppliers = equalization.Items
            .SelectMany(item => item.Quotes)
            .Select(quote => quote.SupplierName)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var items = equalization.Items
            .OrderBy(item => item.Position)
            .Select(item => new SavedEqualizationItemResponseDto(
                item.SourceShoppingItemId,
                item.Name,
                item.Quantity,
                item.Unit,
                item.Quotes
                    .OrderBy(quote => quote.SupplierName, StringComparer.OrdinalIgnoreCase)
                    .Select(quote => new SavedEqualizationQuoteResponseDto(
                        quote.SourceSupplierId,
                        quote.SupplierName,
                        quote.UnitPrice,
                        quote.UnitPrice * item.Quantity,
                        quote.IsLowest))
                    .ToList()))
            .ToList();

        return new SavedEqualizationDetailResponseDto(
            equalization.Id,
            equalization.Code,
            equalization.SourceShoppingListId,
            equalization.ShoppingListName,
            equalization.CreatedByName,
            equalization.CreatedByEmail,
            equalization.BestChoiceTotal,
            equalization.BestCompleteSupplierName,
            equalization.BestCompleteSupplierTotal,
            equalization.EstimatedEconomy,
            equalization.CreatedAtUtc,
            suppliers,
            items);
    }
}
