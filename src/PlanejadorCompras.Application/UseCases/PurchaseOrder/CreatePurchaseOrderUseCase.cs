using System.Net.Mail;
using PlanejadorCompras.Application.Common.Dtos.Requests;
using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.Services;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories;
using PlanejadorCompras.Domain.Repositories.PurchaseOrder;
using PurchaseOrderEntity = PlanejadorCompras.Domain.Entities.PurchaseOrder;

namespace PlanejadorCompras.Application.UseCases.PurchaseOrder;

public sealed class CreatePurchaseOrderUseCase(
    PurchaseOrderDraftService draftService,
    IPurchaseOrderRepository purchaseOrderRepository,
    IPurchaseOrderCodeGenerator codeGenerator,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
{
    private const int MaximumCodeGenerationAttempts = 10;

    public async Task<PurchaseOrderDetailResponseDto> ExecuteAsync(
        CreatePurchaseOrderRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ValidateRequest(request);

        if (await purchaseOrderRepository.ExistsForSourceAsync(
                currentUser.UserId,
                request.ShoppingListId,
                request.SupplierId,
                cancellationToken))
        {
            throw new ConflictException(
                "Ja existe um pedido ativo para esta lista e fornecedor.",
                "purchase_order_already_exists");
        }

        var draft = await draftService.BuildAsync(
            request.ShoppingListId,
            request.SupplierId,
            request.EqualizationId,
            cancellationToken);
        var nowUtc = timeProvider.GetUtcNow().UtcDateTime;
        var code = await GenerateUniqueCodeAsync(nowUtc, cancellationToken);

        var order = PurchaseOrderEntity.Create(
            currentUser.UserId,
            draft.ShoppingListId,
            draft.SupplierId,
            code,
            draft.ShoppingListName,
            draft.SupplierName,
            request.BuyerName,
            request.BuyerEmail,
            request.ExpectedDeliveryDate,
            request.DeliveryAddress,
            request.PaymentTerms,
            request.Notes,
            draft.Items,
            nowUtc,
            draft.EqualizationId);

        await purchaseOrderRepository.AddAsync(order, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return PurchaseOrderResponseMapper.ToDetail(order);
    }

    private async Task<string> GenerateUniqueCodeAsync(
        DateTime nowUtc,
        CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < MaximumCodeGenerationAttempts; attempt++)
        {
            var code = codeGenerator.Generate(nowUtc);

            if (!await purchaseOrderRepository.CodeExistsAsync(code, cancellationToken))
            {
                return code;
            }
        }

        throw new ConflictException(
            "Nao foi possivel gerar um numero unico para o pedido.",
            "purchase_order_code_generation_failed");
    }

    private static void ValidateRequest(CreatePurchaseOrderRequestDto request)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(request.ShoppingListId, Guid.Empty);
        ArgumentOutOfRangeException.ThrowIfEqual(request.SupplierId, Guid.Empty);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.BuyerName);

        EnsureMaximumLength(request.BuyerName, 150, nameof(request.BuyerName));
        EnsureMaximumLength(request.BuyerEmail, 320, nameof(request.BuyerEmail));
        EnsureMaximumLength(request.DeliveryAddress, 500, nameof(request.DeliveryAddress));
        EnsureMaximumLength(request.PaymentTerms, 200, nameof(request.PaymentTerms));
        EnsureMaximumLength(request.Notes, 1000, nameof(request.Notes));

        if (!string.IsNullOrWhiteSpace(request.BuyerEmail)
            && (!MailAddress.TryCreate(request.BuyerEmail.Trim(), out var parsedEmail)
                || !string.Equals(
                    parsedEmail.Address,
                    request.BuyerEmail.Trim(),
                    StringComparison.OrdinalIgnoreCase)))
        {
            throw new BadRequestException(
                "Informe um e-mail valido para o comprador.",
                "purchase_order_invalid_buyer_email");
        }
    }

    private static void EnsureMaximumLength(
        string? value,
        int maximumLength,
        string parameterName)
    {
        if (value?.Trim().Length > maximumLength)
        {
            throw new BadRequestException(
                $"O campo {parameterName} excede o tamanho permitido.",
                "purchase_order_field_too_long");
        }
    }
}
