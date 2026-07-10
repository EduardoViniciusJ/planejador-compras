using PlanejadorCompras.Application.Common.Dtos.Requests;
using PlanejadorCompras.Application.Common.Dtos.Results;
using PlanejadorCompras.Application.Services.Interfaces;

namespace PlanejadorCompras.Application.UseCases.Auth;

public sealed class GoogleAuthorizationCodeLoginUseCase
{
    private readonly IGoogleAuthorizationCodeExchanger _googleAuthorizationCodeExchanger;
    private readonly GoogleLoginUseCase _googleLoginUseCase;

    public GoogleAuthorizationCodeLoginUseCase(
        IGoogleAuthorizationCodeExchanger googleAuthorizationCodeExchanger,
        GoogleLoginUseCase googleLoginUseCase)
    {
        _googleAuthorizationCodeExchanger = googleAuthorizationCodeExchanger;
        _googleLoginUseCase = googleLoginUseCase;
    }

    public async Task<GoogleLoginResultDto> ExecuteAsync(
        GoogleAuthorizationCodeLoginRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Code);

        var idToken = await _googleAuthorizationCodeExchanger.ExchangeForIdTokenAsync(
            request.Code,
            cancellationToken);

        return await _googleLoginUseCase.ExecuteAsync(
            new GoogleLoginRequestDto(idToken),
            cancellationToken);
    }
}
