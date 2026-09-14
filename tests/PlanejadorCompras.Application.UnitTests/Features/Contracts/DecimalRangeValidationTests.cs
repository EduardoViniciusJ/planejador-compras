using System.ComponentModel.DataAnnotations;
using System.Globalization;
using PlanejadorCompras.Application.Features.ItemQuotes.Contracts;
using PlanejadorCompras.Application.Features.ShoppingItems.Contracts;

namespace PlanejadorCompras.Application.UnitTests.Features.Contracts;

public sealed class DecimalRangeValidationTests
{
    [Fact]
    public void ShoppingItemRequestDto_ShouldValidateDecimalRange_WhenCurrentCultureUsesComma()
    {
        WithCulture("pt-BR", () =>
        {
            var dto = new ShoppingItemRequestDto(
                Guid.NewGuid(),
                "Cafe",
                22m,
                "Unidade (un)");

            var results = Validate(dto);

            Assert.Empty(results);
        });
    }

    [Fact]
    public void ItemQuoteRequestDto_ShouldValidateDecimalRange_WhenCurrentCultureUsesComma()
    {
        WithCulture("pt-BR", () =>
        {
            var dto = new ItemQuoteRequestDto(
                Guid.NewGuid(),
                Guid.NewGuid(),
                10.99m);

            var results = Validate(dto);

            Assert.Empty(results);
        });
    }

    private static IReadOnlyList<ValidationResult> Validate(object instance)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(
            instance,
            new ValidationContext(instance),
            results,
            validateAllProperties: true);

        return results;
    }

    private static void WithCulture(string cultureName, Action action)
    {
        var originalCulture = CultureInfo.CurrentCulture;
        var originalUiCulture = CultureInfo.CurrentUICulture;

        try
        {
            var culture = CultureInfo.GetCultureInfo(cultureName);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            action();
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUiCulture;
        }
    }
}
