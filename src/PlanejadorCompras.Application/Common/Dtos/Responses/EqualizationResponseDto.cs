using PlanejadorCompras.Application.Common.Dtos.Models;

namespace PlanejadorCompras.Application.Common.Dtos.Responses;

public record EqualizationResponseDto(
    Guid ShoppingListId,
    IEnumerable<string> Suppliers,
    IEnumerable<EqualizationItemRowDto> Items
);
