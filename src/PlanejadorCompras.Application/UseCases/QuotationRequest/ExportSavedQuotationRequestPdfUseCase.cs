using PlanejadorCompras.Application.Features.Reports.Contracts;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories.QuotationRequest;

namespace PlanejadorCompras.Application.UseCases.QuotationRequest;

public sealed class ExportSavedQuotationRequestPdfUseCase(
    IQuotationRequestRepository repository,
    ICurrentUser currentUser,
    IQuotationRequestPdfExporter exporter)
{
    public async Task<ExportedFileDto> ExecuteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(id, Guid.Empty);
        var request = await repository.GetByIdForUserAsync(
            id,
            currentUser.UserId,
            cancellationToken);
        if (request is null)
        {
            throw new NotFoundException(
                "Quotation request not found.",
                "quotation_request_not_found");
        }

        return await exporter.ExportAsync(
            QuotationRequestResponseMapper.ToReport(request),
            cancellationToken);
    }
}
