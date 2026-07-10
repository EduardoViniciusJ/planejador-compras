using System.ComponentModel.DataAnnotations;

namespace PlanejadorCompras.Application.Common.Dtos.Requests;

public sealed record GoogleAuthorizationCodeLoginRequestDto(
    [Required]
    [MinLength(10)]
    [MaxLength(4096)]
    string Code);
