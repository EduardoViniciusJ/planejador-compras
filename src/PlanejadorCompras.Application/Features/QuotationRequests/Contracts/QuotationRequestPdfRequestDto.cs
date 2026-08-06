using System.ComponentModel.DataAnnotations;

namespace PlanejadorCompras.Application.Features.QuotationRequests.Contracts;

public sealed record QuotationRequestPdfRequestDto(
    DateOnly? ResponseDeadline = null,
    [MaxLength(500)]
    string? DeliveryAddress = null,
    [MaxLength(2000)]
    string? Instructions = null);
