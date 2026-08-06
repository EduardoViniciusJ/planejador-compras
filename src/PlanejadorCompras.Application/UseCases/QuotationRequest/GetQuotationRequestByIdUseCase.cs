using PlanejadorCompras.Application.Features.QuotationRequests.Contracts;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories.QuotationRequest;

namespace PlanejadorCompras.Application.UseCases.QuotationRequest;

public sealed class GetQuotationRequestByIdUseCase(
    IQuotationRequestRepository repository,
    ICurrentUser currentUser)
{
    public async Task<QuotationRequestDetailResponseDto> ExecuteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(id, Guid.Empty);
        var request = await repository.GetByIdForUserAsync(
            id,
            currentUser.UserId,
            cancellationToken);

        return request is null
            ? throw new NotFoundException(
                "Quotation request not found.",
                "quotation_request_not_found")
            : QuotationRequestResponseMapper.ToDetail(request);
    }
}
