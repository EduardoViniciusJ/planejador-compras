using PlanejadorCompras.Application.Common.Dtos.Models;
using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories.ItemQuote;
using PlanejadorCompras.Domain.Repositories.ShoppingItem;

using PlanejadorCompras.Application.UseCases.Interfaces;

namespace PlanejadorCompras.Application.UseCases.ShoppingList;

public sealed class CalculateBestSupplierBudgetUseCase : ICalculateBestSupplierBudgetUseCase
{
    private readonly IShoppingListAccessService _shoppingListAccessService;
    private readonly IShoppingItemRepository _shoppingItemRepository;
    private readonly IItemQuoteRepository _itemQuoteRepository;

    public CalculateBestSupplierBudgetUseCase(
        IShoppingListAccessService shoppingListAccessService,
        IShoppingItemRepository shoppingItemRepository,
        IItemQuoteRepository itemQuoteRepository)
    {
        _shoppingListAccessService = shoppingListAccessService;
        _shoppingItemRepository = shoppingItemRepository;
        _itemQuoteRepository = itemQuoteRepository;
    }

    public async Task<BestSupplierBudgetResponseDto> ExecuteAsync(Guid shoppingListId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(shoppingListId, Guid.Empty);

        await _shoppingListAccessService.GetForCurrentUserAsync(shoppingListId, cancellationToken);

        var items = await _shoppingItemRepository.GetByShoppingListIdAsync(shoppingListId, cancellationToken);
        var quotes = await _itemQuoteRepository.GetByShoppingListIdAsync(shoppingListId, cancellationToken);

        if (!quotes.Any())
        {
            return new BestSupplierBudgetResponseDto(
                shoppingListId,
                null,
                0m,
                Enumerable.Empty<BestSupplierBudgetItemDto>()
            );
        }

        var bestSupplierData = quotes
            .GroupBy(q => q.SupplierName)
            .Select(supplierGroup =>
            {
                decimal supplierTotal = 0m;
                var supplierItems = new List<BestSupplierBudgetItemDto>();

                foreach (var quote in supplierGroup)
                {
                    var item = items.FirstOrDefault(i => i.Id == quote.ShoppingItemId);
                    if (item != null)
                    {
                        var totalItemPrice = quote.UnitPrice * item.Quantity;
                        supplierTotal += totalItemPrice;

                        supplierItems.Add(new BestSupplierBudgetItemDto(
                            item.Id,
                            item.Name,
                            quote.UnitPrice,
                            item.Quantity,
                            totalItemPrice
                        ));
                    }
                }

                return new { SupplierName = supplierGroup.Key, Total = supplierTotal, Items = supplierItems };
            })
            .Where(x => x.Items.Any())
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
