using System.ComponentModel.DataAnnotations;

namespace PlanejadorCompras.Application.Common.Dtos.Requests;

public sealed record GoogleLoginRequestDto(
    [property: Required]
    [property: MinLength(10)]
    string IdToken);
