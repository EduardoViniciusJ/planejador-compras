namespace PlanejadorCompras.Domain.Rules;

public static class ShoppingItemRules
{
    public const int NameMaxLength = 100;
    public const int UnitMaxLength = 20;
    public const int QuantityPrecision = 19;
    public const int QuantityScale = 3;
    public const string MinimumQuantityText = "0.001";
    public const string MaximumQuantityText = "9999999999999999.999";
    public const decimal MinimumQuantity = 0.001m;
    public const decimal MaximumQuantity = 9999999999999999.999m;
}
