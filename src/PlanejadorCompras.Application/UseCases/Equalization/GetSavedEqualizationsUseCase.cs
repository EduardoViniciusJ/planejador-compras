using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories.Equalization;

namespace PlanejadorCompras.Application.UseCases.Equalization;

public sealed class GetSavedEqualizationsUseCase(
    ISavedEqualizationRepository repository,
    ICurrentUser currentUser)
{
    public async Task<PagedResponseDto<SavedEqualizationSummaryResponseDto>> ExecuteAsync(
        string? searchTerm,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (page < 1 || pageSize is < 1 or > 50)
        {
            throw new BadRequestException(
                "Informe uma paginacao valida.",
                "equalization_invalid_pagination");
        }

        var normalizedSearch = string.IsNullOrWhiteSpace(searchTerm)
            ? null
            : searchTerm.Trim();
        var result = await repository.SearchByUserIdAsync(
            currentUser.UserId,
            normalizedSearch,
            page,
            pageSize,
            cancellationToken);

        return new PagedResponseDto<SavedEqualizationSummaryResponseDto>(
            result.Items
                .Select(SavedEqualizationResponseMapper.ToSummary)
                .ToList(),
            page,
            pageSize,
            result.TotalCount);
    }
}
