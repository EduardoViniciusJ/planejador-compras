
namespace PlanejadorCompras.Application.Features.Equalizations.Contracts;

public record EqualizationResponseDto(
    Guid ShoppingListId,
    IEnumerable<string> Suppliers,
    IEnumerable<EqualizationItemRowDto> Items
);
