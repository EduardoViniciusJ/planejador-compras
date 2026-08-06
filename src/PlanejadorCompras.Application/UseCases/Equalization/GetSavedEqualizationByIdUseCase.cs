using PlanejadorCompras.Application.Features.Equalizations.Contracts;
using PlanejadorCompras.Application.Services.Interfaces;

namespace PlanejadorCompras.Application.UseCases.Equalization;

public sealed class GetSavedEqualizationByIdUseCase(
    ISavedEqualizationAccessService accessService)
{
    public async Task<SavedEqualizationDetailResponseDto> ExecuteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var equalization = await accessService.GetForCurrentUserAsync(
            id,
            cancellationToken);

        return SavedEqualizationResponseMapper.ToDetail(equalization);
    }
}
