using PlanejadorCompras.Domain.Repositories.ItemQuote;
using PlanejadorCompras.Domain.Repositories;

namespace PlanejadorCompras.Application.UseCases.ItemQuote;

public sealed class DeleteItemQuoteUseCase
{
    private readonly IItemQuoteRepository _itemQuoteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteItemQuoteUseCase(
        IItemQuoteRepository itemQuoteRepository,
        IUnitOfWork unitOfWork)
    {
        _itemQuoteRepository = itemQuoteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(id, Guid.Empty);

        await _itemQuoteRepository.DeleteAsync(id, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
