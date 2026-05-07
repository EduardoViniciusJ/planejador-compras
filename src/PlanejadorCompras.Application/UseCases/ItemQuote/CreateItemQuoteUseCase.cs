using PlanejadorCompras.Application.Common.Dtos.Requests;
using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Domain.Repositories.ItemQuote;
using PlanejadorCompras.Domain.Repositories;
using ItemQuoteEntity = PlanejadorCompras.Domain.Entities.ItemQuote;

namespace PlanejadorCompras.Application.UseCases.ItemQuote.Create;

public sealed class CreateItemQuoteUseCase
{
    private readonly IItemQuoteRepository _itemQuoteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateItemQuoteUseCase(
        IItemQuoteRepository itemQuoteRepository,
        IUnitOfWork unitOfWork)
    {
        _itemQuoteRepository = itemQuoteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ItemQuoteResponseDto> ExecuteAsync(
        ItemQuoteRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var itemQuote = ItemQuoteEntity.Create(request.ShoppingItemId, request.SupplierName, request.UnitPrice);
        await _itemQuoteRepository.AddAsync(itemQuote, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return new ItemQuoteResponseDto(
            itemQuote.Id,
            itemQuote.ShoppingItemId,
            itemQuote.SupplierName,
            itemQuote.UnitPrice,
            itemQuote.CreatedAt);
    }
}
