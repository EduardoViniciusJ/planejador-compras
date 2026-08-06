using PlanejadorCompras.Domain.Rules;
using PlanejadorCompras.Domain.Validation;

namespace PlanejadorCompras.Domain.Entities;

public sealed class PurchaseOrder
{
    private readonly List<PurchaseOrderItem> _items = [];

    private PurchaseOrder()
    {
    }

    private PurchaseOrder(
        Guid id,
        Guid userId,
        Guid sourceShoppingListId,
        Guid supplierId,
        string code,
        string shoppingListName,
        string supplierName,
        string buyerName,
        string? buyerEmail,
        DateOnly? expectedDeliveryDate,
        string? deliveryAddress,
        string? paymentTerms,
        string? notes,
        DateTime createdAtUtc,
        Guid? sourceEqualizationId)
    {
        Id = id;
        UserId = userId;
        SourceShoppingListId = sourceShoppingListId;
        SupplierId = supplierId;
        Code = code;
        ShoppingListName = shoppingListName;
        SupplierName = supplierName;
        BuyerName = buyerName;
        BuyerEmail = buyerEmail;
        ExpectedDeliveryDate = expectedDeliveryDate;
        DeliveryAddress = deliveryAddress;
        PaymentTerms = paymentTerms;
        Notes = notes;
        SourceEqualizationId = sourceEqualizationId;
        Status = PurchaseOrderStatus.Issued;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public Guid? SourceShoppingListId { get; private set; }

    public Guid? SupplierId { get; private set; }

    public Guid? SourceEqualizationId { get; private set; }

    public string Code { get; private set; } = null!;

    public string ShoppingListName { get; private set; } = null!;

    public string SupplierName { get; private set; } = null!;

    public string BuyerName { get; private set; } = null!;

    public string? BuyerEmail { get; private set; }

    public DateOnly? ExpectedDeliveryDate { get; private set; }

    public string? DeliveryAddress { get; private set; }

    public string? PaymentTerms { get; private set; }

    public string? Notes { get; private set; }

    public PurchaseOrderStatus Status { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }

    public DateTime? CompletedAtUtc { get; private set; }

    public DateTime? CancelledAtUtc { get; private set; }

    public IReadOnlyCollection<PurchaseOrderItem> Items => _items.AsReadOnly();

    public decimal TotalPrice => _items.Sum(item => item.TotalPrice);

    public static PurchaseOrder Create(
        Guid userId,
        Guid sourceShoppingListId,
        Guid supplierId,
        string code,
        string shoppingListName,
        string supplierName,
        string buyerName,
        string? buyerEmail,
        DateOnly? expectedDeliveryDate,
        string? deliveryAddress,
        string? paymentTerms,
        string? notes,
        IReadOnlyCollection<PurchaseOrderItemSnapshot> items,
        DateTime createdAtUtc,
        Guid? sourceEqualizationId = null)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(userId, Guid.Empty);
        ArgumentOutOfRangeException.ThrowIfEqual(sourceShoppingListId, Guid.Empty);
        ArgumentOutOfRangeException.ThrowIfEqual(supplierId, Guid.Empty);
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(shoppingListName);
        ArgumentException.ThrowIfNullOrWhiteSpace(supplierName);
        ArgumentException.ThrowIfNullOrWhiteSpace(buyerName);
        ArgumentNullException.ThrowIfNull(items);

        if (sourceEqualizationId == Guid.Empty)
        {
            throw new ArgumentOutOfRangeException(nameof(sourceEqualizationId));
        }

        if (items.Count == 0)
        {
            throw new ArgumentException(
                "A purchase order must contain at least one item.",
                nameof(items));
        }

        var order = new PurchaseOrder(
            Guid.NewGuid(),
            userId,
            sourceShoppingListId,
            supplierId,
            DomainText.Required(code, PurchaseOrderRules.CodeMaxLength, nameof(code)),
            DomainText.Required(
                shoppingListName,
                PurchaseOrderRules.ShoppingListNameMaxLength,
                nameof(shoppingListName)),
            DomainText.Required(
                supplierName,
                PurchaseOrderRules.SupplierNameMaxLength,
                nameof(supplierName)),
            DomainText.Required(
                buyerName,
                PurchaseOrderRules.BuyerNameMaxLength,
                nameof(buyerName)),
            DomainText.Optional(
                buyerEmail,
                PurchaseOrderRules.BuyerEmailMaxLength,
                nameof(buyerEmail)),
            expectedDeliveryDate,
            DomainText.Optional(
                deliveryAddress,
                PurchaseOrderRules.DeliveryAddressMaxLength,
                nameof(deliveryAddress)),
            DomainText.Optional(
                paymentTerms,
                PurchaseOrderRules.PaymentTermsMaxLength,
                nameof(paymentTerms)),
            DomainText.Optional(notes, PurchaseOrderRules.NotesMaxLength, nameof(notes)),
            createdAtUtc,
            sourceEqualizationId);

        foreach (var item in items)
        {
            order._items.Add(PurchaseOrderItem.Create(order.Id, item));
        }

        return order;
    }

    public void Complete(DateTime nowUtc)
    {
        EnsureCanChangeStatus();
        Status = PurchaseOrderStatus.Completed;
        CompletedAtUtc = nowUtc;
        UpdatedAtUtc = nowUtc;
    }

    public void Cancel(DateTime nowUtc)
    {
        EnsureCanChangeStatus();
        Status = PurchaseOrderStatus.Cancelled;
        CancelledAtUtc = nowUtc;
        UpdatedAtUtc = nowUtc;
    }

    private void EnsureCanChangeStatus()
    {
        if (Status != PurchaseOrderStatus.Issued)
        {
            throw new InvalidOperationException(
                "Only issued purchase orders can be completed or cancelled.");
        }
    }
}
