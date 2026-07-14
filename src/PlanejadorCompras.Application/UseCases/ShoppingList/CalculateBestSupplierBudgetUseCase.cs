using PlanejadorCompras.Application.Common.Dtos.Models;
using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories.ItemQuote;
using PlanejadorCompras.Domain.Repositories.ShoppingItem;

using PlanejadorCompras.Application.UseCases.Interfaces;
using PlanejadorCompras.Domain.Repositories.Supplier;

namespace PlanejadorCompras.Application.UseCases.ShoppingList;

public sealed class CalculateBestSupplierBudgetUseCase : ICalculateBestSupplierBudgetUseCase
{
    private readonly IShoppingListAccessService _shoppingListAccessService;
    private readonly IShoppingItemRepository _shoppingItemRepository;
    private readonly IItemQuoteRepository _itemQuoteRepository;
    private readonly ISupplierRepository _supplierRepository;

    public CalculateBestSupplierBudgetUseCase(
        IShoppingListAccessService shoppingListAccessService,
        IShoppingItemRepository shoppingItemRepository,
        IItemQuoteRepository itemQuoteRepository,
        ISupplierRepository supplierRepository)
    {
        _shoppingListAccessService = shoppingListAccessService;
        _shoppingItemRepository = shoppingItemRepository;
        _itemQuoteRepository = itemQuoteRepository;
        _supplierRepository = supplierRepository;
    }

    public async Task<BestSupplierBudgetResponseDto> ExecuteAsync(Guid shoppingListId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(shoppingListId, Guid.Empty);

        await _shoppingListAccessService.GetForCurrentUserAsync(shoppingListId, cancellationToken);

        var items = await _shoppingItemRepository.GetByShoppingListIdAsync(shoppingListId, cancellationToken);
        var quotes = await _itemQuoteRepository.GetByShoppingListIdAsync(shoppingListId, cancellationToken);
        var suppliers = await _supplierRepository.GetByIdsAsync(
            quotes.Select(quote => quote.SupplierId),
            cancellationToken);
        var supplierNames = suppliers.ToDictionary(supplier => supplier.Id, supplier => supplier.Name);

        if (items.Count == 0 || quotes.Count == 0)
        {
            return new BestSupplierBudgetResponseDto(
                shoppingListId,
                null,
                0m,
                Enumerable.Empty<BestSupplierBudgetItemDto>()
            );
        }

        var itemIds = items.Select(item => item.Id).ToHashSet();

        var bestSupplierData = quotes
            .Where(quote => itemIds.Contains(quote.ShoppingItemId))
            .GroupBy(quote => quote.SupplierId)
            .Select(supplierGroup =>
            {
                var lowestQuotes = supplierGroup
                    .GroupBy(quote => quote.ShoppingItemId)
                    .ToDictionary(
                        group => group.Key,
                        group => group.MinBy(quote => quote.UnitPrice)!);

                var supplierItems = items
                    .Where(item => lowestQuotes.ContainsKey(item.Id))
                    .Select(item =>
                    {
                        var quote = lowestQuotes[item.Id];
                        return new BestSupplierBudgetItemDto(
                            item.Id,
                            item.Name,
                            quote.UnitPrice,
                            item.Quantity,
                            quote.UnitPrice * item.Quantity);
                    })
                    .ToList();

                return new
                {
                    SupplierName = supplierNames[supplierGroup.Key],
                    Total = supplierItems.Sum(item => item.TotalItemPrice),
                    Items = supplierItems
                };
            })
            .Where(result => result.Items.Count == items.Count)
            .OrderBy(x => x.Total)
            .FirstOrDefault();

        return new BestSupplierBudgetResponseDto(
            shoppingListId,
            bestSupplierData?.SupplierName,
            bestSupplierData?.Total ?? 0m,
            bestSupplierData?.Items ?? Enumerable.Empty<BestSupplierBudgetItemDto>()
        );
    }
}
