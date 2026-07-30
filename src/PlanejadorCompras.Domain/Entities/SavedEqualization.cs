namespace PlanejadorCompras.Domain.Entities;

public sealed class SavedEqualization
{
    private readonly List<SavedEqualizationItem> _items = [];

    private SavedEqualization(
        Guid id,
        Guid userId,
        Guid requestId,
        Guid sourceShoppingListId,
        string code,
        string shoppingListName,
        string createdByName,
        string createdByEmail,
        decimal bestChoiceTotal,
        string? bestCompleteSupplierName,
        decimal? bestCompleteSupplierTotal,
        decimal estimatedEconomy,
        DateTime createdAtUtc)
    {
        Id = id;
        UserId = userId;
        RequestId = requestId;
        SourceShoppingListId = sourceShoppingListId;
        Code = code;
        ShoppingListName = shoppingListName;
        CreatedByName = createdByName;
        CreatedByEmail = createdByEmail;
        BestChoiceTotal = bestChoiceTotal;
        BestCompleteSupplierName = bestCompleteSupplierName;
        BestCompleteSupplierTotal = bestCompleteSupplierTotal;
        EstimatedEconomy = estimatedEconomy;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public Guid RequestId { get; private set; }

    public Guid SourceShoppingListId { get; private set; }

    public string Code { get; private set; } = null!;

    public string ShoppingListName { get; private set; } = null!;

    public string CreatedByName { get; private set; } = null!;

    public string CreatedByEmail { get; private set; } = null!;

    public decimal BestChoiceTotal { get; private set; }

    public string? BestCompleteSupplierName { get; private set; }

    public decimal? BestCompleteSupplierTotal { get; private set; }

    public decimal EstimatedEconomy { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public IReadOnlyCollection<SavedEqualizationItem> Items => _items.AsReadOnly();

    public int SupplierCount => _items
        .SelectMany(item => item.Quotes)
        .Select(quote => quote.SourceSupplierId)
        .Distinct()
        .Count();

    public static SavedEqualization Create(
        Guid userId,
        Guid requestId,
        Guid sourceShoppingListId,
        string code,
        string shoppingListName,
        string createdByName,
        string createdByEmail,
        decimal bestChoiceTotal,
        string? bestCompleteSupplierName,
        decimal? bestCompleteSupplierTotal,
        decimal estimatedEconomy,
        IReadOnlyCollection<SavedEqualizationItemSnapshot> items,
        DateTime createdAtUtc)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(userId, Guid.Empty);
        ArgumentOutOfRangeException.ThrowIfEqual(requestId, Guid.Empty);
        ArgumentOutOfRangeException.ThrowIfEqual(sourceShoppingListId, Guid.Empty);
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(shoppingListName);
        ArgumentException.ThrowIfNullOrWhiteSpace(createdByName);
        ArgumentException.ThrowIfNullOrWhiteSpace(createdByEmail);
        ArgumentNullException.ThrowIfNull(items);

        if (items.Count == 0)
        {
            throw new ArgumentException(
                "A saved equalization must contain at least one item.",
                nameof(items));
        }

        if (bestChoiceTotal < 0
            || bestCompleteSupplierTotal < 0
            || estimatedEconomy < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(bestChoiceTotal),
                "Equalization totals cannot be negative.");
        }

        if (bestCompleteSupplierTotal.HasValue
            != !string.IsNullOrWhiteSpace(bestCompleteSupplierName))
        {
            throw new ArgumentException(
                "Complete supplier name and total must be provided together.",
                nameof(bestCompleteSupplierName));
        }

        var equalization = new SavedEqualization(
            Guid.NewGuid(),
            userId,
            requestId,
            sourceShoppingListId,
            code.Trim(),
            shoppingListName.Trim(),
            createdByName.Trim(),
            createdByEmail.Trim(),
            bestChoiceTotal,
            NormalizeOptional(bestCompleteSupplierName),
            bestCompleteSupplierTotal,
            estimatedEconomy,
            createdAtUtc);

        foreach (var item in items)
        {
            equalization._items.Add(SavedEqualizationItem.Create(equalization.Id, item));
        }

        return equalization;
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
