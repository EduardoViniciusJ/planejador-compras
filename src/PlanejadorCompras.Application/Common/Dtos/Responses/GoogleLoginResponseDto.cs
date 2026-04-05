namespace PlanejadorCompras.Application.Common.Dtos.Responses;

public sealed record GoogleLoginResponseDto(Guid UserId, string Email, string Name);
