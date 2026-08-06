using System.ComponentModel.DataAnnotations;
using PlanejadorCompras.Domain.Rules;

namespace PlanejadorCompras.Application.Features.QuotationRequests.Contracts;

public sealed record QuotationRequestPdfRequestDto(
    DateOnly? ResponseDeadline = null,
    [MaxLength(QuotationRequestRules.DeliveryAddressMaxLength)]
    string? DeliveryAddress = null,
    [MaxLength(QuotationRequestRules.InstructionsMaxLength)]
    string? Instructions = null);
