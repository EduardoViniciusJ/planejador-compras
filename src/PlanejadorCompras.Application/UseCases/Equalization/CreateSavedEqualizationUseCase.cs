using PlanejadorCompras.Application.Common.Dtos.Requests;
using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.Services;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Entities;
using PlanejadorCompras.Domain.Repositories;
using PlanejadorCompras.Domain.Repositories.Equalization;
using PlanejadorCompras.Domain.Repositories.ItemQuote;
using PlanejadorCompras.Domain.Repositories.ShoppingItem;
using PlanejadorCompras.Domain.Repositories.Supplier;

namespace PlanejadorCompras.Application.UseCases.Equalization;

public sealed class CreateSavedEqualizationUseCase(
    IShoppingListAccessService shoppingListAccessService,
    IShoppingItemRepository shoppingItemRepository,
    IItemQuoteRepository itemQuoteRepository,
    ISupplierRepository supplierRepository,
    ShoppingListComparisonCalculator comparisonCalculator,
    ISavedEqualizationRepository savedEqualizationRepository,
    ISavedEqualizationCodeGenerator codeGenerator,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
{
    private const int MaximumCodeGenerationAttempts = 10;

    public async Task<SavedEqualizationDetailResponseDto> ExecuteAsync(
        Guid shoppingListId,
        CreateSavedEqualizationRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(shoppingListId, Guid.Empty);
        ArgumentNullException.ThrowIfNull(request);
        ArgumentOutOfRangeException.ThrowIfEqual(request.RequestId, Guid.Empty);

        var existing = await savedEqualizationRepository.GetByRequestIdAsync(
            currentUser.UserId,
            request.RequestId,
            cancellationToken);

        if (existing is not null)
        {
            return SavedEqualizationResponseMapper.ToDetail(existing);
        }

        var shoppingList = await shoppingListAccessService.GetForCurrentUserAsync(
            shoppingListId,
            cancellationToken);
        var items = await shoppingItemRepository.GetByShoppingListIdAsync(
            shoppingListId,
            cancellationToken);
        var quotes = await itemQuoteRepository.GetByShoppingListIdAsync(
            shoppingListId,
            cancellationToken);

        if (items.Count == 0 || quotes.Count == 0)
        {
            throw new BadRequestException(
                "Adicione itens e precos antes de salvar a equalizacao.",
                "equalization_without_prices");
        }

        var suppliers = await supplierRepository.GetByIdsAsync(
            quotes.Select(quote => quote.SupplierId),
            cancellationToken);
        var supplierNames = suppliers.ToDictionary(
            supplier => supplier.Id,
            supplier => supplier.Name);
        var matrix = comparisonCalculator.CalculateEqualization(
            shoppingListId,
            items,
            quotes,
            supplierNames);
        var bestSupplier = comparisonCalculator.CalculateBestSupplierBudget(
            shoppingListId,
            items,
            quotes,
            supplierNames);
        var bestChoiceTotal = matrix.Items.Sum(item =>
            item.Quotes.Any()
                ? item.Quotes.Min(quote => quote.TotalPrice)
                : 0m);
        var hasCompleteSupplier = bestSupplier.BestSupplierName is not null;
        var estimatedEconomy = hasCompleteSupplier
            ? Math.Max(0m, bestSupplier.TotalPrice - bestChoiceTotal)
            : 0m;
        var snapshots = matrix.Items
            .Select((item, position) =>
            {
                var lowestPrice = item.Quotes.Any()
                    ? item.Quotes.Min(quote => quote.UnitPrice)
                    : (decimal?)null;

                return new SavedEqualizationItemSnapshot(
                    item.ShoppingItemId,
                    position,
                    item.ItemName,
                    item.Quantity,
                    item.Unit,
                    item.Quotes
                        .Select(quote => new SavedEqualizationQuoteSnapshot(
                            quote.SupplierId,
                            quote.SupplierName,
                            quote.UnitPrice,
                            quote.UnitPrice == lowestPrice))
                        .ToList());
            })
            .ToList();
        var nowUtc = timeProvider.GetUtcNow().UtcDateTime;
        var code = await GenerateUniqueCodeAsync(nowUtc, cancellationToken);
        var creatorName = string.IsNullOrWhiteSpace(currentUser.Name)
            ? currentUser.Email
            : currentUser.Name;
        var equalization = SavedEqualization.Create(
            currentUser.UserId,
            request.RequestId,
            shoppingListId,
            code,
            shoppingList.Name,
            creatorName,
            currentUser.Email,
            bestChoiceTotal,
            bestSupplier.BestSupplierName,
            hasCompleteSupplier ? bestSupplier.TotalPrice : null,
            estimatedEconomy,
            snapshots,
            nowUtc);

        await savedEqualizationRepository.AddAsync(equalization, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return SavedEqualizationResponseMapper.ToDetail(equalization);
    }

    private async Task<string> GenerateUniqueCodeAsync(
        DateTime nowUtc,
        CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < MaximumCodeGenerationAttempts; attempt++)
        {
            var code = codeGenerator.Generate(nowUtc);

            if (!await savedEqualizationRepository.CodeExistsAsync(
                    code,
                    cancellationToken))
            {
                return code;
            }
        }

        throw new ConflictException(
            "Nao foi possivel gerar um numero unico para a equalizacao.",
            "equalization_code_generation_failed");
    }
}
