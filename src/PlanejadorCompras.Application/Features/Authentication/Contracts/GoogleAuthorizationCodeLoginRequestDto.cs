using System.ComponentModel.DataAnnotations;

namespace PlanejadorCompras.Application.Features.Authentication.Contracts;

public sealed record GoogleAuthorizationCodeLoginRequestDto(
    [Required]
    [MinLength(10)]
    [MaxLength(4096)]
    string Code);
