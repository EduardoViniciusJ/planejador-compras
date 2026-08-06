using PlanejadorCompras.Application.Features.Authentication.Contracts;

namespace PlanejadorCompras.Application.Services.Interfaces;

public interface IJwtTokenService
{
    GenerateAccessTokenResponseDto GenerateAccessToken(GenerateAccessTokenRequestDto request);
}
