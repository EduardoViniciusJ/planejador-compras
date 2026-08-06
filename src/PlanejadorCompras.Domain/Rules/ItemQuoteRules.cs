namespace PlanejadorCompras.Domain.Rules;

public static class ItemQuoteRules
{
    public const int UnitPricePrecision = 18;
    public const int UnitPriceScale = 2;
    public const string MinimumUnitPriceText = "0.01";
    public const string MaximumUnitPriceText = "9999999999999999.99";
    public const decimal MinimumUnitPrice = 0.01m;
    public const decimal MaximumUnitPrice = 9999999999999999.99m;
}
