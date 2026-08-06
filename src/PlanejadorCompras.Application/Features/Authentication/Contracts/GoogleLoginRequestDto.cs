using System.ComponentModel.DataAnnotations;

namespace PlanejadorCompras.Application.Features.Authentication.Contracts;

public sealed record GoogleLoginRequestDto(
    [Required]
    [MinLength(10)]
    [MaxLength(4096)]
    string IdToken);
