using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Domain.Repositories.ItemQuote;

namespace PlanejadorCompras.Application.UseCases.ItemQuote;

public sealed class GetItemQuotesByShoppingItemIdUseCase
{
    private readonly IItemQuoteRepository _itemQuoteRepository;

    public GetItemQuotesByShoppingItemIdUseCase(IItemQuoteRepository itemQuoteRepository)
    {
        _itemQuoteRepository = itemQuoteRepository;
    }

    public async Task<List<ItemQuoteResponseDto>> ExecuteAsync(Guid shoppingItemId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(shoppingItemId, Guid.Empty);

        var itemQuotes = await _itemQuoteRepository.GetByShoppingItemIdAsync(shoppingItemId, cancellationToken);
        return itemQuotes.Select(iq => new ItemQuoteResponseDto(
            iq.Id,
            iq.ShoppingItemId,
            iq.SupplierName,
            iq.UnitPrice,
            iq.CreatedAt)).ToList();
    }
}
