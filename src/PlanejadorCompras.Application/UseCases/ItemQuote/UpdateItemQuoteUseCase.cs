using PlanejadorCompras.Application.Common.Dtos.Requests;
using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Domain.Repositories.ItemQuote;
using PlanejadorCompras.Domain.Repositories;

namespace PlanejadorCompras.Application.UseCases.ItemQuote;

public sealed class UpdateItemQuoteUseCase
{
    private readonly IItemQuoteRepository _itemQuoteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateItemQuoteUseCase(
        IItemQuoteRepository itemQuoteRepository,
        IUnitOfWork unitOfWork)
    {
        _itemQuoteRepository = itemQuoteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ItemQuoteResponseDto?> ExecuteAsync(
        Guid id,
        ItemQuoteRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentOutOfRangeException.ThrowIfEqual(id, Guid.Empty);

        var itemQuote = await _itemQuoteRepository.GetByIdAsync(id, cancellationToken);
        if (itemQuote is null)
        {
            return null;
        }

        await _itemQuoteRepository.UpdateAsync(itemQuote, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return new ItemQuoteResponseDto(
            itemQuote.Id,
            itemQuote.ShoppingItemId,
            itemQuote.SupplierName,
            itemQuote.UnitPrice,
            itemQuote.CreatedAt);
    }
}
