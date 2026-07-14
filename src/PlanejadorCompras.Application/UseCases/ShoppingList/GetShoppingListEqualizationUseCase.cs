using PlanejadorCompras.Application.Common.Dtos.Models;
using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories.ItemQuote;
using PlanejadorCompras.Domain.Repositories.ShoppingItem;
using PlanejadorCompras.Application.UseCases.Interfaces;
using PlanejadorCompras.Domain.Repositories.Supplier;

namespace PlanejadorCompras.Application.UseCases.ShoppingList;

public sealed class GetShoppingListEqualizationUseCase : IGetShoppingListEqualizationUseCase
{
    private readonly IShoppingListAccessService _shoppingListAccessService;
    private readonly IShoppingItemRepository _shoppingItemRepository;
    private readonly IItemQuoteRepository _itemQuoteRepository;
    private readonly ISupplierRepository _supplierRepository;

    public GetShoppingListEqualizationUseCase(
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

    public async Task<EqualizationResponseDto> ExecuteAsync(Guid shoppingListId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(shoppingListId, Guid.Empty);

        await _shoppingListAccessService.GetForCurrentUserAsync(shoppingListId, cancellationToken);

        var items = await _shoppingItemRepository.GetByShoppingListIdAsync(shoppingListId, cancellationToken);
        var quotes = await _itemQuoteRepository.GetByShoppingListIdAsync(shoppingListId, cancellationToken);
        var supplierEntities = await _supplierRepository.GetByIdsAsync(
            quotes.Select(quote => quote.SupplierId),
            cancellationToken);
        var supplierNames = supplierEntities.ToDictionary(supplier => supplier.Id, supplier => supplier.Name);

        if (!items.Any())
        {
            return new EqualizationResponseDto(
                shoppingListId,
                Enumerable.Empty<string>(),
                Enumerable.Empty<EqualizationItemRowDto>()
            );
        }

        if (!quotes.Any())
        {
            var emptyItemRows = items.Select(item => new EqualizationItemRowDto(
                item.Id,
                item.Name,
                item.Quantity,
                item.Unit,
                Enumerable.Empty<EqualizationQuoteDto>()
            )).ToList();

            return new EqualizationResponseDto(
                shoppingListId,
                Enumerable.Empty<string>(),
                emptyItemRows
            );
        }

        var suppliers = quotes
            .Select(quote => supplierNames[quote.SupplierId])
            .Distinct()
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var itemRows = items.Select(item =>
        {
            var itemQuotes = quotes
                .Where(q => q.ShoppingItemId == item.Id)
                .GroupBy(quote => quote.SupplierId)
                .Select(group => group.MinBy(quote => quote.UnitPrice)!)
                .Select(quote => new EqualizationQuoteDto(
                    supplierNames[quote.SupplierId],
                    quote.UnitPrice,
                    quote.UnitPrice * item.Quantity));

            return new EqualizationItemRowDto(
                item.Id,
                item.Name,
                item.Quantity,
                item.Unit,
                itemQuotes
            );
        }).ToList();

        return new EqualizationResponseDto(
            shoppingListId,
            suppliers,
            itemRows
        );
    }
}
