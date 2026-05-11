using PlanejadorCompras.Application.Common.Dtos.Requests;
using PlanejadorCompras.Application.Common.Dtos.Responses;

namespace PlanejadorCompras.Application.Services.Interfaces;

public interface IJwtTokenService
{
    GenerateAccessTokenResponseDto GenerateAccessToken(GenerateAccessTokenRequestDto request);
}
