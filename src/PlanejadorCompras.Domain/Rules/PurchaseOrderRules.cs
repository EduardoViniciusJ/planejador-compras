namespace PlanejadorCompras.Domain.Rules;

public static class PurchaseOrderRules
{
    public const int CodeMaxLength = 32;
    public const int ShoppingListNameMaxLength = ShoppingListRules.NameMaxLength;
    public const int SupplierNameMaxLength = 200;
    public const int BuyerNameMaxLength = 150;
    public const int BuyerEmailMaxLength = 320;
    public const int DeliveryAddressMaxLength = 500;
    public const int PaymentTermsMaxLength = 200;
    public const int NotesMaxLength = 1000;
    public const int ItemNameMaxLength = 200;
    public const int ItemUnitMaxLength = 50;
}
