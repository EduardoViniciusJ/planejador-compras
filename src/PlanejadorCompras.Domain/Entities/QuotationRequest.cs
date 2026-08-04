namespace PlanejadorCompras.Domain.Entities;

public sealed class QuotationRequest
{
    private readonly List<QuotationRequestItem> _items = [];

    private QuotationRequest()
    {
    }

    private QuotationRequest(
        Guid id,
        Guid userId,
        Guid sourceShoppingListId,
        string code,
        string shoppingListName,
        string? description,
        string buyerName,
        string buyerEmail,
        DateOnly? responseDeadline,
        string? deliveryAddress,
        string? instructions,
        DateTime createdAtUtc)
    {
        Id = id;
        UserId = userId;
        SourceShoppingListId = sourceShoppingListId;
        Code = code;
        ShoppingListName = shoppingListName;
        Description = Normalize(description);
        BuyerName = buyerName;
        BuyerEmail = buyerEmail;
        ResponseDeadline = responseDeadline;
        DeliveryAddress = Normalize(deliveryAddress);
        Instructions = Normalize(instructions);
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid? SourceShoppingListId { get; private set; }
    public string Code { get; private set; } = null!;
    public string ShoppingListName { get; private set; } = null!;
    public string? Description { get; private set; }
    public string BuyerName { get; private set; } = null!;
    public string BuyerEmail { get; private set; } = null!;
    public DateOnly? ResponseDeadline { get; private set; }
    public string? DeliveryAddress { get; private set; }
    public string? Instructions { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public IReadOnlyCollection<QuotationRequestItem> Items => _items.AsReadOnly();

    public static QuotationRequest Create(
        Guid userId,
        Guid sourceShoppingListId,
        string shoppingListName,
        string? description,
        string buyerName,
        string buyerEmail,
        DateOnly? responseDeadline,
        string? deliveryAddress,
        string? instructions,
        IReadOnlyCollection<QuotationRequestItemSnapshot> items,
        DateTime createdAtUtc)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(userId, Guid.Empty);
        ArgumentOutOfRangeException.ThrowIfEqual(sourceShoppingListId, Guid.Empty);
        ArgumentException.ThrowIfNullOrWhiteSpace(shoppingListName);
        ArgumentException.ThrowIfNullOrWhiteSpace(buyerName);
        ArgumentException.ThrowIfNullOrWhiteSpace(buyerEmail);
        ArgumentNullException.ThrowIfNull(items);
        if (items.Count == 0) throw new ArgumentException("A quotation request must contain items.", nameof(items));

        var id = Guid.NewGuid();
        var code = $"SC-{createdAtUtc.Year}-{id:N}"[..14].ToUpperInvariant();
        var request = new QuotationRequest(
            id,
            userId,
            sourceShoppingListId,
            code,
            shoppingListName.Trim(),
            description,
            buyerName.Trim(),
            buyerEmail.Trim().ToLowerInvariant(),
            responseDeadline,
            deliveryAddress,
            instructions,
            createdAtUtc);

        var position = 0;
        foreach (var item in items)
        {
            request._items.Add(QuotationRequestItem.Create(request.Id, position, item));
            position++;
        }

        return request;
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed record QuotationRequestItemSnapshot(
    Guid SourceShoppingItemId,
    string Name,
    decimal Quantity,
    string Unit);
