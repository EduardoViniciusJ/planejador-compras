using PlanejadorCompras.Application.Features.QuotationRequests.Contracts;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Application.UseCases.ShoppingList;
using PlanejadorCompras.Domain.Entities;
using PlanejadorCompras.Domain.Repositories;
using PlanejadorCompras.Domain.Repositories.QuotationRequest;
using QuotationRequestEntity = PlanejadorCompras.Domain.Entities.QuotationRequest;

namespace PlanejadorCompras.Application.UseCases.QuotationRequest;

public sealed class CreateQuotationRequestUseCase(
    GetShoppingListDetailUseCase getShoppingListDetailUseCase,
    IQuotationRequestRepository repository,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
{
    public async Task<QuotationRequestDetailResponseDto> ExecuteAsync(
        Guid shoppingListId,
        QuotationRequestPdfRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(shoppingListId, Guid.Empty);
        ArgumentNullException.ThrowIfNull(request);

        var detail = await getShoppingListDetailUseCase.ExecuteAsync(
            shoppingListId,
            cancellationToken);

        if (detail.Items.Count == 0)
        {
            throw new BadRequestException(
                "Adicione ao menos um item antes de emitir a solicitacao de cotacao.",
                "quotation_request_empty_list");
        }

        var nowUtc = timeProvider.GetUtcNow().UtcDateTime;
        var today = DateOnly.FromDateTime(nowUtc);
        if (request.ResponseDeadline.HasValue && request.ResponseDeadline.Value < today)
        {
            throw new BadRequestException(
                "O prazo de resposta nao pode estar no passado.",
                "quotation_request_invalid_deadline");
        }

        var buyerName = string.IsNullOrWhiteSpace(currentUser.Name)
            ? currentUser.Email
            : currentUser.Name;
        var entity = QuotationRequestEntity.Create(
            currentUser.UserId,
            shoppingListId,
            detail.Name,
            detail.Description,
            buyerName,
            currentUser.Email,
            request.ResponseDeadline,
            request.DeliveryAddress,
            request.Instructions,
            detail.Items
                .Select(item => new QuotationRequestItemSnapshot(
                    item.Id,
                    item.Name,
                    item.Quantity,
                    item.Unit))
                .ToList(),
            nowUtc);

        await repository.AddAsync(entity, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return QuotationRequestResponseMapper.ToDetail(entity);
    }
}
