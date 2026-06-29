using System.ComponentModel.DataAnnotations;

namespace PlanejadorCompras.Application.Common.Dtos.Requests;

public sealed record GoogleLoginRequestDto(
    [Required]
    [MinLength(10)]
    string IdToken);
