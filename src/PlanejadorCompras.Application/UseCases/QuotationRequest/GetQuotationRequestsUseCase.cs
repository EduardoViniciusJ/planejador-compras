using PlanejadorCompras.Application.Features.QuotationRequests.Contracts;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories.QuotationRequest;

namespace PlanejadorCompras.Application.UseCases.QuotationRequest;

public sealed class GetQuotationRequestsUseCase(
    IQuotationRequestRepository repository,
    ICurrentUser currentUser)
{
    public async Task<IReadOnlyCollection<QuotationRequestSummaryResponseDto>> ExecuteAsync(
        CancellationToken cancellationToken = default) =>
        (await repository.GetByUserIdAsync(currentUser.UserId, cancellationToken))
            .Select(QuotationRequestResponseMapper.ToSummary)
            .ToList();
}
