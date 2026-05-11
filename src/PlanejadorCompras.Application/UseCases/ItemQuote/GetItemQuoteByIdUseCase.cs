using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Domain.Repositories.ItemQuote;

namespace PlanejadorCompras.Application.UseCases.ItemQuote;

public sealed class GetItemQuoteByIdUseCase
{
    private readonly IItemQuoteRepository _itemQuoteRepository;

    public GetItemQuoteByIdUseCase(IItemQuoteRepository itemQuoteRepository)
    {
        _itemQuoteRepository = itemQuoteRepository;
    }

    public async Task<ItemQuoteResponseDto> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(id, Guid.Empty);

        var itemQuote = await _itemQuoteRepository.GetByIdAsync(id, cancellationToken);
        if (itemQuote is null)
        {
            throw new NotFoundException("Item quote not found.", "item_quote_not_found");
        }

        return new ItemQuoteResponseDto(
            itemQuote.Id,
            itemQuote.ShoppingItemId,
            itemQuote.SupplierName,
            itemQuote.UnitPrice,
            itemQuote.CreatedAt);
    }
}
