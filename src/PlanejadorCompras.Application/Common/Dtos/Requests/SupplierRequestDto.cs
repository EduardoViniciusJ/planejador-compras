using System.ComponentModel.DataAnnotations;

namespace PlanejadorCompras.Application.Common.Dtos.Requests;

public sealed record SupplierRequestDto(
    [Required]
    [MinLength(1)]
    [MaxLength(100)]
    string Name);
