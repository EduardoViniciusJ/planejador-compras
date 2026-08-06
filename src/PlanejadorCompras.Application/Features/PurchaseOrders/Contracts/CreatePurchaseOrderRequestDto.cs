using System.ComponentModel.DataAnnotations;
using PlanejadorCompras.Domain.Rules;

namespace PlanejadorCompras.Application.Features.PurchaseOrders.Contracts;

public sealed record CreatePurchaseOrderRequestDto(
    [Required]
    Guid ShoppingListId,
    [Required]
    Guid SupplierId,
    [Required]
    [MinLength(1)]
    [MaxLength(PurchaseOrderRules.BuyerNameMaxLength)]
    string BuyerName,
    [EmailAddress]
    [MaxLength(PurchaseOrderRules.BuyerEmailMaxLength)]
    string? BuyerEmail,
    DateOnly? ExpectedDeliveryDate,
    [MaxLength(PurchaseOrderRules.DeliveryAddressMaxLength)]
    string? DeliveryAddress,
    [MaxLength(PurchaseOrderRules.PaymentTermsMaxLength)]
    string? PaymentTerms,
    [MaxLength(PurchaseOrderRules.NotesMaxLength)]
    string? Notes,
    Guid? EqualizationId = null);
