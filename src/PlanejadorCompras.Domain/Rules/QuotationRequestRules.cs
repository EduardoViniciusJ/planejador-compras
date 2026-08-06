namespace PlanejadorCompras.Domain.Rules;

public static class QuotationRequestRules
{
    public const int CodeMaxLength = 32;
    public const int ShoppingListNameMaxLength = ShoppingListRules.NameMaxLength;
    public const int DescriptionMaxLength = ShoppingListRules.DescriptionMaxLength;
    public const int BuyerNameMaxLength = 150;
    public const int BuyerEmailMaxLength = 320;
    public const int DeliveryAddressMaxLength = 500;
    public const int InstructionsMaxLength = 2000;
    public const int ItemNameMaxLength = ShoppingItemRules.NameMaxLength;
    public const int ItemUnitMaxLength = ShoppingItemRules.UnitMaxLength;
}
